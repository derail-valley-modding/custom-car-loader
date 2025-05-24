using CCL.Importer.Patches;
using CCL.Importer.Processing;
using CCL.Importer.Types;
using CCL.Types;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityModManagerNet;

namespace CCL.Importer
{
    public static class CarManager
    {
        public static readonly List<CCL_CarType> CustomCarTypes = new();

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
                DV.Globals.G.Types.RecalculateCaches();
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

            int loaded = 0;

            foreach (var car in pack.Cars)
            {
                loaded += LoadCar(car) ? 1 : 0;
            }

            CCLPlugin.Log("Processing extra models...");
            foreach (var model in pack.ExtraModels)
            {
                ModelProcessor.DoBasicProcessing(model);
            }

            // Load paints.
            if (pack.PaintSubstitutions.Length > 0)
            {
                CCLPlugin.Log("Loading paints...");
                PaintLoader.LoadSubstitutions(pack.PaintSubstitutions);
            }
            // Generate procedural materials.
            if (pack.ProceduralMaterials != null)
            {
                CCLPlugin.Log("Generating materials...");
                ProceduralMaterialGenerator.Generate(pack.ProceduralMaterials);
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

                if (!string.IsNullOrEmpty(car.LicenseID))
                {
                    if (DV.Globals.G.Types.TryGetGeneralLicense(car.LicenseID, out var license))
                    {
                        foreach (var item in carType.liveries)
                        {
                            item.requiredLicense = license;
                        }
                    }
                    else
                    {
                        CCLPlugin.Warning($"Failed to find license for {car.id} ({car.LicenseID}), car will not require license to use");
                    }
                }

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
    }
}
