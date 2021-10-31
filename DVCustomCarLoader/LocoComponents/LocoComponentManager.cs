using System;
using System.Collections.Generic;
using CCL_GameScripts;
using CCL_GameScripts.Attributes;
using CCL_GameScripts.CabControls;
using DV.CabControls.Spec;
using DV.MultipleUnit;
using DVCustomCarLoader.LocoComponents;
using HarmonyLib;
using UnityEngine;

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

        private static readonly Dictionary<TrainCarType, GameObject> InteriorPrefabCache =
            new Dictionary<TrainCarType, GameObject>();

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

            // add brake compressor, rate is set in controller start
            var train = prefab.GetComponent<TrainCar>();
            train.hasCompressor = simParams.AirCompressorRate > 0;

            var drivingForce = prefab.AddComponent<DrivingForce>();
            ApplyDrivingForceParams(drivingForce, simParams);

            SetupHorn(prefab);

            prefab.AddComponent<CustomLocoSimDiesel>();
            prefab.AddComponent<CustomDieselSimEvents>();
            prefab.AddComponent<DamageControllerCustomDiesel>();
            var locoController = prefab.AddComponent<CustomLocoControllerDiesel>();

            // setup multiple unit module
            var muHoses = prefab.GetComponentsInChildren<CouplingHoseMultipleUnitAdapter>(true);
            if( muHoses.Length == 0 )
            {
                Main.Warning($"Could not find MU hose adapters for diesel engine");
            }
            else
            {
                var muModule = prefab.AddComponent<MultipleUnitModule>();
                foreach( var muHoseAdapter in muHoses )
                {
                    Vector3 hoseOffset = prefab.transform.InverseTransformPoint(muHoseAdapter.transform.position);
                    if( hoseOffset.z > 0 )
                    {
                        // front coupling
                        muModule.frontCableAdapter = muHoseAdapter;
                    }
                    else
                    {
                        muModule.rearCableAdapter = muHoseAdapter;
                    }
                }

                Main.Log("Added multiple unit module");
            }

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

        public static void SetupCabComponents( GameObject interior, LocoParamsType locoType )
        {
            CreateComponentsFromProxies(interior);
            CreateCopiedControls(interior);

            if( locoType == LocoParamsType.DieselElectric )
            {
                interior.AddComponent<CustomFuseController>();
            }

            // add controller/"brain" components
            interior.AddComponent<CustomCabInputController>();
        }

        public static void CreateComponentsFromProxies( GameObject root )
        {
            var allInitSpecs = root.GetComponentsInChildren<ComponentInitSpec>(true);

            foreach( var compSpec in allInitSpecs )
            {
                GameObject controlObject = compSpec.gameObject;
                if( compSpec is ControlSetupBase control && !((compSpec is IApertureTrackable a) && a.TrackAsAperture)  )
                {
#if DEBUG
                    Main.Log($"Add input relay to {controlObject.name}");
#endif
                    var inputRelay = controlObject.AddComponent<CabInputRelay>();
                    inputRelay.Binding = control.InputBinding;
                    inputRelay.MapMin = control.MappedMinimum;
                    inputRelay.MapMax = control.MappedMaximum;
                    inputRelay.AbsPosition = control.UseAbsoluteMappedValue;
                }
                
                var realComp = InitSpecManager.CreateRealComponent(compSpec);
            }
        }

        public static void CreateCopiedControls( GameObject root )
        {
            var allCopySpecs = root.GetComponentsInChildren<CopiedCabDevice>(true);

            foreach( var copySpec in allCopySpecs )
            {
                GameObject copierAttachedObject = copySpec.gameObject;
                (BaseTrainCarType carType, string sourceObjName) = copySpec.GetSourceObject();

                GameObject sourceInterior = GetTrainCarInterior((TrainCarType)carType);
                if( !sourceInterior ) continue;

                Transform sourceChild = sourceInterior.transform.Find(sourceObjName);
                if( sourceChild )
                {
                    GameObject newControl;
                    if( copySpec.ReplaceThisObject )
                    {
                        newControl = UnityEngine.Object.Instantiate(sourceChild.gameObject, copierAttachedObject.transform.parent);
                        newControl.transform.localPosition = copierAttachedObject.transform.localPosition;
                        newControl.transform.localRotation = copierAttachedObject.transform.localRotation;
                    }
                    else
                    {
                        newControl = UnityEngine.Object.Instantiate(sourceChild.gameObject, copierAttachedObject.transform);
                        newControl.transform.localPosition = Vector3.zero;
                        newControl.transform.localRotation = Quaternion.identity;
                    }

                    // copy over existing name for clarity
                    newControl.name = copierAttachedObject.name;

                    // copy over any proxied fields
                    if( copySpec is IProxyScript proxyScript )
                    {
                        Type targetType = AccessTools.TypeByName(proxyScript.TargetTypeName);
                        if( targetType != null )
                        {
                            object proxyTarget = newControl.GetComponent(targetType);
                            if( proxyTarget != null )
                            {
                                proxyScript.ApplyProxyFields(proxyTarget, AccessTools.TypeByName, Main.Warning);
                            }
                        }
                        else
                        {
                            Main.Error($"Failed to find proxy target {proxyScript.TargetTypeName} for copy script {copySpec.GetType().Name}");
                        }
                    }

                    InitSpecManager.ExecuteStaticAfterCopy(copySpec, newControl);

                    if( copySpec.ReplaceThisObject )
                    {
                        UnityEngine.Object.Destroy(copierAttachedObject);
                    }
                }
            }
        }

        public static GameObject GetTrainCarInterior( TrainCarType carType )
        {
            if( !InteriorPrefabCache.TryGetValue(carType, out GameObject interior) )
            {
                var prefab = CarTypes.GetCarPrefab(carType);
                if( !prefab )
                {
                    Main.Error($"CarType {carType} has missing prefab");
                    return null;
                }

                TrainCar car = prefab.GetComponent<TrainCar>();
                if( !car )
                {
                    Main.Warning($"Couldn't find TrainCar on carType {prefab.name}");
                    return null;
                }

                if( car.interiorPrefab == null || !car.interiorPrefab )
                {
                    Main.Warning($"TrainCar on carType {prefab.name} doesn't have an interiorPrefab assigned");
                    return null;
                }

                interior = car.interiorPrefab;
                InteriorPrefabCache.Add(carType, interior);
            }

            return interior;
        }

        public static void SetupHorn( GameObject prefab )
        {
            Horn newHorn = prefab.AddComponent<Horn>();
            newHorn.playHornAt = prefab.transform;
        }
    }
}
