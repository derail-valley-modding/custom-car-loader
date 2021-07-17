using System;
using System.Collections.Generic;
using System.Linq;
using DV.MultipleUnit;
using UnityEngine;
using DVCustomCarLoader.LocoComponents;
using CCL_GameScripts;
using HarmonyLib;
using CCL_GameScripts.CabControls;

namespace DVCustomCarLoader
{
    public static class LocoComponentManager
    {
        public static void AddLocoSimulation( GameObject prefab, SimParamsBase simParams )
        {
            switch( simParams.SimType )
            {
                case LocoParamsType.DieselElectric:
                    AddDieselSimulation(prefab, (SimParamsDiesel)simParams);
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

        public static void AddDieselSimulation( GameObject prefab, SimParamsDiesel simParams )
        {
            var dmgConfig = prefab.GetComponent<DamageConfigDiesel>();
            if( !dmgConfig )
            {
                Main.Error($"Loco prefab {prefab.name} is missing diesel damage config, skipping sim setup");
                return;
            }

            var drivingForce = prefab.AddComponent<DrivingForce>();
            ApplyDrivingForceParams(drivingForce, simParams);

            prefab.AddComponent<CustomLocoSimDiesel>();
            prefab.AddComponent<CustomDieselSimEvents>();
            prefab.AddComponent<DamageControllerCustomDiesel>();
            //prefab.AddComponent<MultipleUnitModule>();
            var locoController = prefab.AddComponent<CustomLocoControllerDiesel>();
            locoController.drivingForce = drivingForce;

            Main.Log($"Added diesel electric simulation to {prefab.name}");
        }

        private static void ApplyDrivingForceParams( DrivingForce driver, SimParamsBase simParams )
        {
            driver.frictionCoeficient = simParams.FrictionCoefficient;
            driver.preventWheelslip = simParams.PreventWheelslip;
            driver.sandCoefMax = simParams.SandCoefficient;
            driver.slopeCoeficientMultiplier = simParams.SlopeCoefficientMultiplier;
            driver.wheelslipToFrictionModifierCurve = simParams.WheelslipToFrictionModifier;
        }

        public static void SetupCabInput( GameObject interior )
        {
            var cabParams = interior.GetComponent<CabInputSetup>();
            if( cabParams )
            {
                CreateComponentsFromProxies(interior);
                interior.AddComponent<CustomCabInput>();
            }
            else
            {
                Main.Warning("Loco has an interior prefab, but no cab input setup");
            }
        }

        public static void CreateComponentsFromProxies( GameObject root )
        {
            var allInitSpecs = root.GetComponentsInChildren<ComponentInitSpec>();
            foreach( var compSpec in allInitSpecs )
            {
                compSpec.CreateRealComponent(AccessTools.TypeByName, Main.Warning);
            }
        }
    }

    [HarmonyPatch(typeof(TrainCar), "LoadInterior")]
    public static class TrainCar_LoadInterior_Patch
    {
        public static void Postfix( GameObject ___loadedInterior )
        {
            if( !___loadedInterior.activeSelf )
            {
                ___loadedInterior.gameObject.SetActive(true);
                Main.Log($"Activating interior on {___loadedInterior.gameObject.name}");
            }
        }
    }
}
