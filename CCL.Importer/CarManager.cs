using CCL.Importer.Patches;
using CCL.Importer.Processing;
using CCL.Importer.Types;
using CCL.Importer.WorkTrains;
using CCL.Types;
using DV;
using DV.ThingTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;

namespace CCL.Importer
{
    public static class CarManager
    {
        private const int IdMappingStart = -1000;

        public static readonly List<CCL_CarType> CustomCarTypes = new();
        public static readonly Dictionary<TrainCarLivery, TrainCarLivery[]> Trainsets = new();
        public static readonly List<string> LoadFailures = new();

        internal static readonly Dictionary<string, int> CurrentMapping = new();
        internal static readonly HashSet<TrainCarType> AddedValues = new();

        private static int s_tempId = IdMappingStart;

        public static void ScanLoadedMods()
        {
            foreach (var mod in UnityModManager.modEntries)
            {
                if (mod.Active)
                {
                    LoadCarDefinitions(mod);
                }
            }
        }

        public static void HandleModToggled(UnityModManager.ModEntry modEntry, bool newState)
        {
            if (newState)
            {
                LoadCarDefinitions(modEntry);
            }
        }

        private static void LoadCarDefinitions(UnityModManager.ModEntry mod)
        {
            int loaded = 0;

            foreach (string file in Directory.EnumerateFiles(mod.Path, ExporterConstants.EXPORTED_BUNDLE_NAME, SearchOption.AllDirectories))
            {
                var bundle = AssetBundle.LoadFromFile(file);

                if (bundle == null)
                {
                    CCLPlugin.Error("Failed to load bundle!");
                    continue;
                }

                foreach (var pack in bundle.LoadAllAssets<CustomCarPack>())
                {
                    loaded += LoadPack(pack);
                }

                bundle.Unload(false);
                Mapper.ClearComponentCache();
            }

            if (loaded > 0)
            {
                CCLPlugin.LogVerbose($"Loaded {loaded} cars from {mod.Path}");
                Globals.G.Types.RecalculateCaches();
            }
        }

        private static int LoadPack(CustomCarPack pack)
        {
            var version = new Version(pack.ExporterVersion);

            if (version > ExporterConstants.ExporterVersion)
            {
                CCLPlugin.Error($"Pack {pack.PackId} was built with a newer version of CCL:\n" +
                    $"Current Version = {ExporterConstants.ExporterVersion}\n" +
                    $"Pack Version = {version}");
                return 0;
            }
            else if (version < ExporterConstants.MinimumCompatibleVersion)
            {
                CCLPlugin.Error($"Pack {pack.PackId} was built with an incompatible version of CCL:\n" +
                    $"Minimum Version = {ExporterConstants.MinimumCompatibleVersion}\n" +
                    $"Pack Version = {version}");
                return 0;
            }

            pack.AfterImport();

            // Generate procedural materials.
            if (pack.ProceduralMaterials != null)
            {
                CCLPlugin.Log("Generating materials...");
                ProceduralMaterialGenerator.Generate(pack.ProceduralMaterials);
            }
            // Load paints.
            if (pack.PaintSubstitutions.Length > 0)
            {
                CCLPlugin.Log("Loading paints...");
                PaintLoader.LoadSubstitutions(pack.PaintSubstitutions);
            }

            int loaded = 0;

            foreach (var car in pack.Cars)
            {
                if (LoadCar(car))
                {
                    loaded++;
                }
                else
                {
                    LoadFailures.Add($"{car.id} ({pack.PackId})");
                }
            }

            CCLPlugin.Log("Processing extra models...");
            foreach (var model in pack.ExtraModels)
            {
                ModelProcessor.DoBasicProcessing(model);
            }

            InjectTranslations(pack);

            return loaded;
        }

        private static void InjectTranslations(CustomCarPack pack)
        {
            if (pack.ExtraTranslations == null) return;

            CCLPlugin.Log("Injection custom translations...");
            if (pack.ExtraTranslations.Terms != null)
            {
                foreach (var term in pack.ExtraTranslations.Terms)
                {
                    CCLPlugin.Translations.AddTranslations(term.Term, term.Data);
                }
            }

            if (!string.IsNullOrEmpty(pack.ExtraTranslations.CSV_Url))
            {
                CCLPlugin.Translations.AddTranslationsFromWebCsv(pack.ExtraTranslations.CSV_Url!);
            }
        }

        private static bool LoadCar(CustomCarType car)
        {
            try
            {
                // Ensure no duplicate IDs ever load, entire game dies otherwise.
                if (Globals.G.Types.carTypes.Any(x => x.id == car.id))
                {
                    CCLPlugin.Error($"Failed to load car {car.id}, car ID already ingame");
                    return false;
                }

                foreach (var livery in car.liveries)
                {
                    if (Globals.G.Types.Liveries.Any(x => x.id == livery.id))
                    {
                        CCLPlugin.Error($"Failed to load car {car.id}, livery ID '{livery.id}' already ingame");
                        return false;
                    }
                }

                car.AfterImport();

                var carType = ScriptableObject.CreateInstance<CCL_CarType>();
                Mapper.M.Map(car, carType);

                CarTypeInjector.SetupTypeLinks(carType);

                // Finalize model & inject
                if (!FinalizeCarTypePrefabs(carType))
                {
                    CCLPlugin.Error($"Failed to wrangle prefab for {car.id}");
                    return false;
                }

                if (!CarTypeInjector.RegisterCarType(carType))
                {
                    CCLPlugin.Error($"Failed to register car type for {car.id}");
                    return false;
                }

                SetupLicenses(car, carType);
                SetupWorkTrains(carType);
                SetupTempIds(carType);

                // Create the HUD if it exists.
                if (car.HUDLayout != null)
                {
                    CCLPlugin.Log("Generating HUD from layout settings...");
                    carType.hudPrefab = HUDGenerator.CreateHUD(car.HUDLayout);

                    if (car.HUDLayout.HUDType == CCL.Types.HUD.VanillaHUDLayout.BaseHUD.Custom)
                    {
                        carType.hudPrefab.name = $"HUD-{car.id}";
                    }
                }

                if (car.IsActualSteamLocomotive)
                {
                    foreach (var item in car.liveries)
                    {
                        CarTypesPatches.SteamLocomotiveIds.Add(item.id);
                    }
                }

                CustomCarTypes.Add(carType);
                CCLPlugin.Log($"Successfully loaded car type {car.id}");
            }
            catch (Exception e)
            {
                CCLPlugin.Error($"Error loading car from {car.id}:\n{e}");
                return false;
            }

            return true;
        }

        private static bool FinalizeCarTypePrefabs(CCL_CarType carType)
        {
            foreach (var livery in carType.Variants)
            {
                if (!FinalizeLiveryPrefab(livery))
                {
                    return false;
                }
            }

            if (carType.SimAudioPrefab != null)
            {
                carType.SimAudioPrefab = ModelProcessor.CreateModifiablePrefab(carType.SimAudioPrefab);
                ModelProcessor.DoBasicProcessing(carType.SimAudioPrefab);
            }

            return true;
        }

        private static bool FinalizeLiveryPrefab(CCL_CarVariant livery)
        {
            var processor = new ModelProcessor(livery);
            processor.ExecuteSteps();
            return true;
        }

        private static void SetupLicenses(CustomCarType car, CCL_CarType carType)
        {
            if (!string.IsNullOrEmpty(car.GeneralLicense))
            {
                if (Globals.G.Types.TryGetGeneralLicense(car.GeneralLicense, out var license))
                {
                    foreach (var item in carType.liveries)
                    {
                        item.requiredLicense = license;
                    }
                }
                else
                {
                    CCLPlugin.Warning($"Failed to find general license for {car.id} ({car.GeneralLicense}), car will not require license to use");
                }
            }

            List<JobLicenseType_v2> jobLicenses = new();

            foreach (var id in car.JobLicenses)
            {
                if (Globals.G.Types.TryGetJobLicense(id, out var license))
                {
                    jobLicenses.Add(license);
                }
                else
                {
                    CCLPlugin.Warning($"Failed to find job license for {car.id} ({car.GeneralLicense}), car will not require this license to use");
                }
            }

            carType.requiredJobLicenses = jobLicenses.ToArray();
        }

        private static void SetupWorkTrains(CCL_CarType carType)
        {
            foreach (var item in carType.Variants)
            {
                if (item.UnlockableAsWorkTrain)
                {
                    WorkTrainPurchaseHandler.WorkTrainLiveries.Add(item);
                }
            }
        }

        private static void SetupTempIds(CCL_CarType carType)
        {
            foreach (var item in carType.liveries)
            {
                item.v1 = (TrainCarType)(--s_tempId);
            }
        }

        internal static void AddTrainsets(CCL_CarType car)
        {
            foreach(var livery in car.Variants)
            {
                // No trainset for single vehicles.
                if (livery.TrainsetLiveries.Length < 2) continue;

                List<TrainCarLivery> liveries = new();

                foreach (var id in livery.TrainsetLiveries)
                {
                    if (DV.Globals.G.Types.TryGetLivery(id, out var match))
                    {
                        liveries.Add(match);
                    }
                    else
                    {
                        CCLPlugin.Error($"Could not find livery {id} for trainset of {livery.id}");
                    }
                }

                Trainsets.Add(livery, liveries.ToArray());
            }
        }

        internal static void LoadMapping(SaveGameData data)
        {
            CurrentMapping.Clear();
            AddedValues.Clear();

            if (data == null) return;

            var keys = data.GetStringArray(SaveConstants.LIVERY_MAPPING_KEY);
            var values = data.GetIntArray(SaveConstants.LIVERY_MAPPING_VALUE);

            // No data.
            if (keys == null || values == null)
            {
                CCLPlugin.Warning("No mapping data in save file, ignoring mapping load");
                return;
            }

            if (keys.Length != values.Length)
            {
                CCLPlugin.Error("Error loading mapping data from save file: keys and values don't match!");
            }

            int length = Mathf.Min(keys.Length, values.Length);

            for (int i = 0; i < length; i++)
            {
                CurrentMapping.Add(keys[i], values[i]);
                AddedValues.Add((TrainCarType)values[i]);
            }
        }

        internal static void SaveMapping(SaveGameData data)
        {
            data.SetStringArray(SaveConstants.LIVERY_MAPPING_KEY, CurrentMapping.Keys.ToArray());
            data.SetIntArray(SaveConstants.LIVERY_MAPPING_VALUE, CurrentMapping.Values.ToArray());
        }

        internal static void ApplyMapping()
        {
            int lowest = IdMappingStart;

            foreach (var item in CurrentMapping)
            {
                lowest = Mathf.Min(lowest, item.Value);
            }

            int newTypes = 0;

            foreach (var type in CustomCarTypes)
            {
                foreach (var livery in type.liveries)
                {
                    if (!CurrentMapping.TryGetValue(livery.id, out var value))
                    {
                        value = --lowest;
                        CurrentMapping.Add(livery.id, value);
                        newTypes++;
                    }

                    livery.v1 = (TrainCarType)value;
                    AddedValues.Add(livery.v1);

                    if (livery.prefab != null)
                    {
                        var car = livery.prefab.GetComponentInChildren<TrainCar>();

                        if (car != null)
                        {
                            // Why is this. Why. Why does it need a separate field for it.
                            car.carType = livery.v1;
                        }
                    }
                }
            }

            Globals.G.Types.RecalculateCaches();
            CCLPlugin.Log($"Mappings applied: {AddedValues.Count}/{CurrentMapping.Count} (new: {newTypes}), lowest value is {lowest}");
        }

        /// <summary>
        /// Returns an array of ordered liveries that should always be together (i.e. loco + tender).
        /// </summary>
        /// <param name="car">The livery that may or may not belong to a trainset.</param>
        /// <returns>An array of liveries if there is a trainset, or an empty array if there is not.</returns>
        /// <remarks>Base game car liveries are ignored by this method.</remarks>
        public static TrainCarLivery[] GetTrainsetForLivery(TrainCarLivery car)
        {
            if (Trainsets.TryGetValue(car, out var set)) return set;

            return Array.Empty<TrainCarLivery>();
        }

        /// <summary>
        /// Returns an array of ordered train cars that belong together (i.e. loco + tender).
        /// </summary>
        /// <param name="car">The car instance that may or may not belong to a trainset.</param>
        /// <returns>An array of train cars if there is a trainset, or an empty array if there is not.</returns>
        /// <remarks>Base game car liveries are ignored by this method.</remarks>
        public static TrainCar[] GetInstancedTrainset(TrainCar car)
        {
            var trainset = GetTrainsetForLivery(car.carLivery);

            // Same rules as the other method.
            if (trainset.Length < 2) return Array.Empty<TrainCar>();

            int check = trainset.Length;

            while (check > 0)
            {
                // If the current car isn't the starting one for the sequence, advance 1 car forwards.
                // Check variable decreases and means we only check at most the # of cars in the set.
                if (!MatchingLiverySequence(car, trainset))
                {
                    // If we cannot advance more, stop.
                    if (car.frontCoupler == null || car.frontCoupler.coupledTo == null) break;

                    car = car.frontCoupler.coupledTo.train;
                    check--;
                    continue;
                }

                var result = new TrainCar[trainset.Length];

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = car;

                    // Stop walking through at the end.
                    if (i == result.Length - 1) break;

                    // It is safe to iterate as MatchingLiverySequence needs to match the length.
                    car = car.rearCoupler.coupledTo.train;
                }

                return result;
            }

            return Array.Empty<TrainCar>();

            static bool MatchingLiverySequence(TrainCar? car, TrainCarLivery[] liveries)
            {
                for (int i = 0; i < liveries.Length; i++)
                {
                    // Check if the current car livery matches with the order.
                    if (car == null || car.carLivery != liveries[i]) return false;

                    // Get the next car, or null if there is nothing coupled.
                    car = car.rearCoupler != null && car.rearCoupler.coupledTo != null ? car.rearCoupler.coupledTo.train : null;
                }

                return true;
            }
        }
    }
}
