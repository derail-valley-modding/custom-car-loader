using System;
using System.Collections.Generic;
using System.Linq;
using DV.MultipleUnit;
using UnityEngine;

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

            prefab.AddComponent<DieselLocoSimulation>();
            prefab.AddComponent<DieselLocoSimulationEvents>();
            prefab.AddComponent<DamageControllerDiesel>();
            //prefab.AddComponent<MultipleUnitModule>();

            var controller = prefab.AddComponent<LocoControllerDiesel>();
            var baseController = basePrefab.GetComponent<LocoControllerDiesel>();
            controller.tractionTorqueCurve = baseController.tractionTorqueCurve;
            controller.brakePowerCurve = baseController.brakePowerCurve;
        }
    }
}
