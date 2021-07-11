using System;
using System.Collections.Generic;
using System.Linq;
using DV.MultipleUnit;
using UnityEngine;
using DVCustomCarLoader.LocoComponents;

namespace DVCustomCarLoader
{
    public static class LocoComponentManager
    {
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
