using System;
using System.Collections.Generic;
using System.Linq;
using DV.MultipleUnit;
using UnityEngine;
using DVCustomCarLoader.LocoComponents;
using CCL_GameScripts;

namespace DVCustomCarLoader
{
    public static class LocoComponentManager
    {
        public static void AddLocoSimulation( GameObject prefab, SimParamsBase simParams )
        {
            switch( simParams.SimType )
            {
                case LocoParamsType.DieselElectric:
                    AddDieselSimulation(prefab);
                    break;

                default:
                    break;
            }
        }

        // Order to add components:
        // - Simulation
        // - SimulationEvents
        // - DamageController
        // - MultipleUnitModule
        // - LocoController

        public static void AddDieselSimulation( GameObject prefab )
        {
            GameObject basePrefab = CarTypes.GetCarPrefab(TrainCarType.LocoDiesel);

            prefab.AddComponent<CustomLocoSimDiesel>();
            prefab.AddComponent<CustomDieselSimEvents>();
            prefab.AddComponent<DamageControllerCustomDiesel>();
            //prefab.AddComponent<MultipleUnitModule>();

            var controller = prefab.AddComponent<CustomLocoControllerDiesel>();
            var baseController = basePrefab.GetComponent<LocoControllerDiesel>();
            controller.tractionTorqueCurve = baseController.tractionTorqueCurve;
            controller.brakePowerCurve = baseController.brakePowerCurve;
        }
    }
}
