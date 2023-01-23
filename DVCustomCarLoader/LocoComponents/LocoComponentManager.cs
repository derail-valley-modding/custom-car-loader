using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CCL_GameScripts;
using CCL_GameScripts.Attributes;
using CCL_GameScripts.CabControls;
using CCL_GameScripts.Effects;
using DV.CabControls;
using DV.CabControls.Spec;
using DV.MultipleUnit;
using DVCustomCarLoader.LocoComponents;
using DVCustomCarLoader.LocoComponents.DieselElectric;
using DVCustomCarLoader.LocoComponents.Steam;
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

                case LocoParamsType.Steam:
                    AddSteamSimulation(prefab, (SimParamsSteam)simParams);
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
                Main.LogVerbose($"Could not find MU hose adapters for diesel engine");
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
                locoController.muModule = muModule;

                Main.LogVerbose("Added multiple unit module");
            }

            locoController.drivingForce = drivingForce;

            var sparkSetup = prefab.GetComponentInChildren<WheelSparkSetup>();
            if (sparkSetup)
            {
                Effects.ParticleInitializer.AddWheelSparks(prefab, sparkSetup, LocoSimTemplate.DE6);
            }

            Main.LogVerbose($"Added diesel electric simulation to {prefab.name}");
        }

        public static void AddSteamSimulation(GameObject prefab, SimParamsSteam simParams)
        {
            // add brake compressor, rate is set in controller start
            var train = prefab.GetComponent<TrainCar>();
            train.hasCompressor = simParams.AirCompressorRate > 0;

            var drivingForce = prefab.AddComponent<DrivingForce>();
            ApplyDrivingForceParams(drivingForce, simParams);

            prefab.AddComponent<CustomLocoSimSteam>();
            prefab.AddComponent<CustomLocoSimEventsSteam>();
            prefab.AddComponent<CustomDamageControllerSteam>();

            var locoController = prefab.AddComponent<CustomLocoControllerSteam>();
            locoController.drivingForce = drivingForce;

            if (!simParams.IsTankLoco)
            {
                prefab.AddComponent<CustomTenderAutoCouple>();
            }

            var sparkSetup = prefab.GetComponentInChildren<WheelSparkSetup>();
            if (sparkSetup)
            {
                Effects.ParticleInitializer.AddWheelSparks(prefab, sparkSetup, LocoSimTemplate.SH282);
            }

            prefab.AddComponent<CustomChuffController>();

            Main.LogVerbose($"Added steam simulation to {prefab.name}");
        }

        private static void ApplyDrivingForceParams( DrivingForce driver, SimParamsBase simParams )
        {
            driver.preventWheelslip = simParams.PreventWheelslip;
            driver.sandCoefMax = simParams.SandCoefficient;
            driver.slopeCoeficientMultiplier = simParams.SlopeCoefficientMultiplier;
            driver.wheelslipToFrictionModifierCurve = simParams.WheelslipToFrictionModifier;
        }

        public static void SetupCabComponents(GameObject interior, SimParamsBase simParams)
        {
            CreateComponentsFromProxies(interior);
            CreateCopiedControls(interior);

            if (simParams.SimType == LocoParamsType.DieselElectric)
            {
                interior.AddComponent<CustomFuseController>();
            }
            else if (simParams.SimType == LocoParamsType.Steam)
            {
                SetupFirebox(interior, simParams as SimParamsSteam);
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
                    Main.LogVerbose($"Add input relay to {controlObject.name}");
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

        public static void MakeDoorsCollidable(GameObject prefab)
        {
            var doors = prefab.GetComponentsInChildren<LeverSetup>()
                .Where(l => l.TrackAsDoor);

            foreach (var door in doors)
            {
                var collider = door.gameObject.GetComponentInChildren<BoxCollider>();
                if (collider)
                {
                    var playerCollideRoot = new GameObject("playerCollision");
                    playerCollideRoot.transform.parent = collider.transform.parent;
                    playerCollideRoot.transform.localPosition = collider.transform.localPosition;
                    playerCollideRoot.transform.localRotation = collider.transform.localRotation;
                    playerCollideRoot.transform.localScale = collider.transform.localScale;

                    var playerCollide = playerCollideRoot.AddComponent<BoxCollider>();
                    playerCollide.center = collider.center;
                    playerCollide.size = collider.size;

                    playerCollideRoot.SetLayersRecursive("Train_Walkable");
                }
            }
        }

        public static void SetInteriorLayers(GameObject interior)
        {
            ModelUtil.SetLayersRecursiveAndExclude(interior, DVLayer.Interactable, DVLayer.Train_Walkable);

            var coalPiles = interior.GetComponentsInChildren<ShovelCoalPile>().Cast<Component>();
            var coalTargets = interior.GetComponentsInChildren<NonPhysicsCoalTarget>();

            foreach (var component in coalPiles.Concat(coalTargets))
            {
                component.gameObject.layer = LayerMask.NameToLayer("Train_Interior");
            }
        }

        private static void SetupFirebox(GameObject interior, SimParamsSteam simParams)
        {
            var fireSetup = interior.GetComponentInChildren<FireboxSetup>();
            if (fireSetup)
            {
                GameObject oldFireObj = fireSetup.gameObject;

                var steamInterior = GetTrainCarInterior(TrainCarType.LocoSteamHeavy);
                var baseFireTform = steamInterior.transform.Find(CarPartNames.FIREBOX_ROOT);
                var newFireObj = GameObject.Instantiate(baseFireTform.gameObject, oldFireObj.transform.parent);
                newFireObj.transform.localPosition = oldFireObj.transform.localPosition;
                newFireObj.transform.localRotation = oldFireObj.transform.localRotation;
                newFireObj.transform.localScale = oldFireObj.transform.localScale;

                var baseFire = newFireObj.GetComponent<Fire>();
                var newFire = newFireObj.AddComponent<CustomFire>();

                newFire.InitializeFromOther(baseFire);
                var scaleV = newFireObj.transform.localScale;

                // scale flames emitter
                var fireParticlesTform = newFireObj.transform.Find(CarPartNames.FIREBOX_FLAMES);
                var fireParticles = fireParticlesTform.GetComponent<ParticleSystem>();
                newFire.fireObj = fireParticles.gameObject;

                var newPos = fireParticlesTform.localPosition + new Vector3(0, 0, 0.3f);
                fireParticlesTform.localPosition = newPos;

                var main = fireParticles.main;
                main.duration *= 0.5f;

                // scale spark emitter
                var sparksTform = newFireObj.transform.Find(CarPartNames.FIREBOX_SPARKS);
                var sparks = sparksTform.GetComponent<ParticleSystem>();
                var sparksShape = sparks.shape;
                sparksShape.radius *= Mathf.Min(scaleV.x, scaleV.z);
                newFire.sparksObj = sparks.gameObject;

                if (fireSetup.ReplacementCoalMesh)
                {
                    var coalRoot = newFireObj.transform.Find(CarPartNames.FIREBOX_COAL);
                    GameObject.Destroy(coalRoot.GetComponentInChildren<MeshFilter>(true));
                    GameObject.Destroy(coalRoot.GetComponentInChildren<MeshRenderer>(true));

                    fireParticlesTform.localPosition = Vector3.zero;
                    var customFireTform = fireSetup.ReplacementCoalMesh.transform.Find("fire");
                    if (customFireTform)
                    {
                        fireParticlesTform.SetParent(customFireTform, false);
                    }
                    else
                    {
                        fireParticlesTform.SetParent(fireSetup.ReplacementCoalMesh, false);
                    }
                }
                else
                {
                    var fpMain = fireParticles.main;
                    fpMain.startSizeXMultiplier = scaleV.x;
                    fpMain.startSizeYMultiplier = scaleV.y;
                    fpMain.startSizeZMultiplier = scaleV.z;

                    foreach (var indicator in newFireObj.GetComponents<Indicator>())
                    {
                        var relay = newFireObj.AddComponent<IndicatorRelay>();
                        relay.Initialize(new OutputBinding(SimEventType.FireboxLevel), indicator);
                        indicator.maxValue = simParams.FireboxCapacity;
                    }
                }

                GameObject.Destroy(baseFire);
                GameObject.Destroy(oldFireObj);

                if (fireSetup.HideDefaultWalls)
                {
                    var wallsRoot = newFireObj.transform.Find(CarPartNames.FIREBOX_MESH);
                    GameObject.Destroy(wallsRoot.GetComponentInChildren<MeshFilter>(true));
                    GameObject.Destroy(wallsRoot.GetComponentInChildren<MeshRenderer>(true));
                }
            }
        }

        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            var copy = destination.AddComponent<T>();
            FieldInfo[] fields = typeof(T).GetFields();

            foreach (var field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }

            return copy;
        }
    }
}
