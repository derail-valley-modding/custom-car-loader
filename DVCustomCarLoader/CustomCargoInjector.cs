using CCL_GameScripts;
using DV;
using DV.Logic.Job;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DVCustomCarLoader
{
    public static class CustomCargoInjector
    {
        private static readonly Dictionary<string, CargoType> idToCargoType = new Dictionary<string, CargoType>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, CustomCargo> idToCustomCargo = new Dictionary<string, CustomCargo>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<CargoType, CustomCargo> cargoTypeToDefinition = new Dictionary<CargoType, CustomCargo>();

        public static IEnumerable<CargoType> CustomCargoTypes => cargoTypeToDefinition.Keys;

        private static readonly Dictionary<CargoType, float> cargoToDamagePrice;
        private static readonly Dictionary<CargoType, float> cargoToEnvironmentDamagePrice;
        private static readonly Dictionary<CargoType, string> cargoShortDisplayName;
        private static readonly Dictionary<CargoType, string> cargoSpecificDisplayName;

        private static readonly Queue<CargoRoutingInfo> lateInitRoutingInfo = new Queue<CargoRoutingInfo>();

        public static bool TryGetCargoTypeById(string id, out CargoType type) => idToCargoType.TryGetValue(id, out type);
        public static CargoType CargoTypeById(string id) => idToCargoType[id];
        public static bool TryGetCustomCargoById(string id, out CustomCargo cargo) => idToCustomCargo.TryGetValue(id, out cargo);
        public static CustomCargo CustomCargoById(string id) => idToCustomCargo[id];

        public static bool TryGetCustomCargoByType(CargoType type, out CustomCargo cargo) => cargoTypeToDefinition.TryGetValue(type, out cargo);
        public static CustomCargo CustomCarByType(CargoType type) => cargoTypeToDefinition[type];

        public static bool IsCustomTypeRegistered(CargoType cargoType) => cargoTypeToDefinition.ContainsKey(cargoType);
        public static bool IsCustomTypeRegistered(string identifier) => idToCargoType.ContainsKey(identifier);


        private static readonly List<CustomCargo> pendingDefinitions = new List<CustomCargo>();

        public static void AddCargoDefinitionToPending(CustomCargo definition)
        {
            var alreadyPending = pendingDefinitions.FirstOrDefault(d => d.Identifier == definition.Identifier);
            if ((alreadyPending != null) || IsCustomTypeRegistered(definition.Identifier))
            {
                Main.Warning($"Cargo {definition.Identifier} is defined more than once, skipping extra definition");
                return;
            }

            pendingDefinitions.Add(definition);
        }

        public static bool RequestCargoTypeFinalization(string cargoId)
        {
            if (IsCustomTypeRegistered(cargoId)) return true;

            var pending = pendingDefinitions.FirstOrDefault(d => d.Identifier == cargoId);
            if (pending == null)
            {
                return false;
            }
            else
            {
                InjectCargoDefinition(pending);
                return true;
            }
        }

        static CustomCargoInjector()
        {
            AccessTools.Field(typeof(ResourceTypes), "cargoToFullCargoDamagePrice")
                .SaveTo(out cargoToDamagePrice);

            AccessTools.Field(typeof(ResourceTypes), "cargoToFullEnvironmentDamagePrice")
                .SaveTo(out cargoToEnvironmentDamagePrice);

            AccessTools.Field(typeof(CargoTypes), "cargoShortDisplayName")
                .SaveTo(out cargoShortDisplayName);

            AccessTools.Field(typeof(CargoTypes), "cargoSpecificDisplayName")
                .SaveTo(out cargoSpecificDisplayName);
        }

        public static void LoadDefinitions()
        {
            string cargoPath = Path.Combine(Main.ModEntry.Path, "Cargo");
            Main.LogVerbose("Loading cargo definitions from " + cargoPath);

            foreach (string directory in Directory.EnumerateDirectories(cargoPath))
            {
                string definitionFile = Path.Combine(cargoPath, directory, CarJSONKeys.CARGO_JSON_FILE);

                if (File.Exists(definitionFile))
                {
                    try
                    {
                        using (StreamReader reader = File.OpenText(definitionFile))
                        {
                            string jsonText = reader.ReadToEnd();
                            
                            var json = new JSONObject(jsonText);
                            var packItems = json.list;
                            foreach (var packItem in packItems)
                            {
                                var newCargo = CustomCargo.Import(packItem);

                                if (newCargo != null)
                                {
                                    Main.LogVerbose($"Loaded cargo definition {newCargo.Identifier}");
                                    AddCargoDefinitionToPending(newCargo);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Main.Error(ex.Message);
                    }
                }
            }
        }

        private static void InjectCargoDefinition(CustomCargo cargo)
        {
            CargoType newType = BaseInjector.GenerateUniqueType<CargoType>(cargo.Identifier, IsCustomTypeRegistered);

            idToCargoType.Add(cargo.Identifier, newType);
            idToCustomCargo.Add(cargo.Identifier, cargo);
            cargoTypeToDefinition.Add(newType, cargo);

            cargoShortDisplayName.Add(newType, cargo.ShortName);
            cargoSpecificDisplayName.Add(newType, cargo.SpecificName);

            CargoTypes.cargoTypeToCargoMassPerUnit.Add(newType, cargo.MassPerUnit);
            cargoToDamagePrice.Add(newType, cargo.ValuePerUnit);
            cargoToEnvironmentDamagePrice.Add(newType, cargo.EnvironmentDamagePrice);

            if (LogicController.Exists && LogicController.Instance.initialized)
            {
                InjectCargoRoutingInfo(newType, cargo.Identifier, cargo.Sources, cargo.Destinations);
            }
            else
            {
                lateInitRoutingInfo.Enqueue(new CargoRoutingInfo(newType, cargo.Identifier, cargo.Sources, cargo.Destinations));
            }

            Main.LogVerbose($"Injected custom cargo type {cargo.Identifier} - {cargo.MassPerUnit}kg, ${cargo.ValuePerUnit}");
        }

        private static void InjectCargoRoutingInfo(CargoType cargo, string name, StationYard sources, StationYard destinations)
        {
            // inject into job generation rules
            var srcStations = new List<StationController>();
            var destStations = new List<StationController>();

            foreach (string yardId in sources.YardIds())
            {
                if (!LogicController.Instance.YardIdToStationController.TryGetValue(yardId, out var controller))
                {
                    Main.Error($"Invalid source station {yardId} for cargo {name}");
                    continue;
                }

                srcStations.Add(controller);
            }

            foreach (string destYardId in destinations.YardIds())
            {
                if (!LogicController.Instance.YardIdToStationController.TryGetValue(destYardId, out var destController))
                {
                    Main.Error($"Invalid destination station {destYardId} for cargo {name}");
                    continue;
                }

                destStations.Add(destController);
            }

            var cargos = new List<CargoType>(1) { cargo };

            foreach (var srcStation in srcStations)
            {
                srcStation.proceduralJobsRuleset.outputCargoGroups.Add(new CargoGroup(cargos, destStations));
                AddCargoToWarehouse(srcStation, cargo);
            }

            foreach (var destStation in destStations)
            {
                destStation.proceduralJobsRuleset.inputCargoGroups.Add(new CargoGroup(cargos, srcStations));
                AddCargoToWarehouse(destStation, cargo);
            }
        }

        private static readonly FieldInfo cargoTextField = AccessTools.Field(typeof(WarehouseMachineController), "supportedCargoTypesText");
        private static readonly MethodInfo displayIdleMethod = AccessTools.Method(typeof(WarehouseMachineController), "DisplayIdleText");

        private static void AddCargoToWarehouse(StationController station, CargoType cargo)
        {
            Main.LogVerbose($"Add cargo to station warehouse: {station?.name} - {cargo.GetCargoName()}");
            var loadMachine = station.logicStation.yard.WarehouseMachines.First();
            if (loadMachine != null)
            {
                var loadMachineController = WarehouseMachineController.allControllers.First(c => c.warehouseMachine == loadMachine);
                loadMachine.SupportedCargoTypes.Add(cargo);
                loadMachineController.supportedCargoTypes.Add(cargo);

                string cargoText = cargoTextField.GetValue(loadMachineController) as string;
                cargoTextField.SetValue(loadMachineController, cargoText + cargo.GetCargoName() + '\n');
                displayIdleMethod.Invoke(loadMachineController, new object[0]);
            }
        }

        public static void OnLogicControllerInitialized()
        {
            while (lateInitRoutingInfo.Count > 0)
            {
                var routingInfo = lateInitRoutingInfo.Dequeue();
                InjectCargoRoutingInfo(routingInfo.Cargo, routingInfo.Name, routingInfo.Sources, routingInfo.Destinations);
            }
        }

        private struct CargoRoutingInfo
        {
            public CargoType Cargo;
            public string Name;
            public StationYard Sources;
            public StationYard Destinations;

            public CargoRoutingInfo(CargoType cargo, string name, StationYard sources, StationYard destinations)
            {
                Cargo = cargo;
                Name = name;
                Sources = sources;
                Destinations = destinations;
            }
        }
    }

    [HarmonyPatch]
    public static class CustomCargoPatches
    {
        [HarmonyPatch(typeof(LicenseManager), nameof(LicenseManager.GetRequiredLicensesForCargoTypes))]
        [HarmonyPostfix]
        public static void GetRequiredLicensesForCargoTypes(List<CargoType> cargoTypes, ref JobLicenses __result)
        {
            foreach (CargoType cargo in cargoTypes)
            {
                if (CustomCargoInjector.TryGetCustomCargoByType(cargo, out CustomCargo definition))
                {
                    __result |= (JobLicenses)definition.RequiredLicense;
                }
            }
        }
    }

    [HarmonyPatch(typeof(CommsRadioCargoLoader))]
    public static class CommsRadioCargoLoader_Patches
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void CommsRadioCargoLoader_Enable_Postfix(List<CargoType> ___cargoTypesToLoad)
        {
            if (___cargoTypesToLoad != null)
            {
                Main.LogVerbose("Injecting custom cargos to comms radio loader");
                ___cargoTypesToLoad.AddRange(CustomCargoInjector.CustomCargoTypes);
            }
            else
            {
                Main.Error("Failed to connect to radio cargo loader");
            }
        }
    }
}
