using CCL.Importer.Processing;
using CCL.Importer.Types;
using CCL.Types;
using CCL.Types.Components;
using DV.Damage;
using DV.ModularAudioCar;
using DV.Simulation.Controllers;
using DV.Simulation.Ports;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using LocoSim.Implementations.Wheels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Importer
{
    public static class CarAudioProcessor
    {
        private static Dictionary<VanillaAudioSystem, LayeredAudio> s_audioSystems = new Dictionary<VanillaAudioSystem, LayeredAudio>();

        private static GameObject? s_audioDE2;
        private static GameObject? s_audioDE6;
        private static GameObject? s_audioDH4;
        private static GameObject? s_audioDM3;
        private static GameObject? s_audioS060;
        private static GameObject? s_audioS282;
        private static GameObject? s_audioMicroshunter;

        private static GameObject AudioDE2 =>
            Extensions.GetCached(ref s_audioDE2, () => TrainCarType.LocoShunter.ToV2().parentType.audioPrefab);
        private static GameObject AudioDE6 =>
            Extensions.GetCached(ref s_audioDE6, () => TrainCarType.LocoDiesel.ToV2().parentType.audioPrefab);
        private static GameObject AudioDH4 =>
            Extensions.GetCached(ref s_audioDH4, () => TrainCarType.LocoDH4.ToV2().parentType.audioPrefab);
        private static GameObject AudioDM3 =>
            Extensions.GetCached(ref s_audioDM3, () => TrainCarType.LocoDM3.ToV2().parentType.audioPrefab);
        private static GameObject AudioS060 =>
            Extensions.GetCached(ref s_audioS060, () => TrainCarType.LocoS060.ToV2().parentType.audioPrefab);
        private static GameObject AudioS282 =>
            Extensions.GetCached(ref s_audioS282, () => TrainCarType.LocoSteamHeavy.ToV2().parentType.audioPrefab);
        private static GameObject AudioMicroshunter =>
            Extensions.GetCached(ref s_audioMicroshunter, () => TrainCarType.LocoMicroshunter.ToV2().parentType.audioPrefab);

        private static bool NeedsWheelsAudioModule(CCL_CarType carType)
        {
            var prefab = carType.liveries[0].prefab;
            var poweredWheels = prefab.GetComponentInChildren<PoweredWheelsManager>();
            var damage = prefab.GetComponent<DamageController>();

            return poweredWheels || (damage && (damage.wheels != null));
        }

        private static bool NeedsCustomAudio(CCL_CarType carType) =>
            (carType.SimAudioPrefab || NeedsWheelsAudioModule(carType));

        public static void InjectAudioPrefabs(TrainComponentPool pool)
        {
            foreach (var carType in CarManager.CustomCarTypes)
            {
                if (carType.audioPrefab) return;

                if (!NeedsCustomAudio(carType)) continue;

                carType.audioPoolSize = 10;

                var newAudioFab = ModelProcessor.CreateModifiablePrefab(pool.defaultAudioPrefab);
                var modularAudio = newAudioFab.GetComponent<CarModularAudio>();

                if (carType.SimAudioPrefab)
                {
                    var simAudioFab = ModelProcessor.CreateModifiablePrefab(carType.SimAudioPrefab);

                    simAudioFab.transform.SetParent(newAudioFab.transform, false);
                    simAudioFab.SetActive(true);

                    // setup optional rain
                    var rainLocation = simAudioFab.transform.Find(CarPartNames.Audio.RAIN_DUMMY_TRANSFORM);
                    if (rainLocation)
                    {
                        var rainAudio = AudioDM3.transform.Find(CarPartNames.Audio.RAIN_MODULE).gameObject;
                        var newRainAudio = Object.Instantiate(rainAudio, newAudioFab.transform);
                        newRainAudio.transform.position = rainLocation.position;

                        var rainAudioModule = newRainAudio.GetComponent<RainAudioModule>();
                        modularAudio.audioModules.Add(rainAudioModule);

                        Object.Destroy(rainLocation.gameObject);
                    }

                    ProcessAudioCopies(simAudioFab);

                    // setup sim module
                    var simAudio = simAudioFab.AddComponent<SimAudioModule>();
                    simAudioFab.GetComponent<LayeredAudioSimReadersController>().OnValidate();
                    simAudioFab.GetComponent<AudioClipSimReadersController>().OnValidate();

                    // add to main audio controller
                    modularAudio.audioModules.Add(simAudio);

                    CCLPlugin.Log($"Created audio prefab for {carType.id}");
                }

                if (NeedsWheelsAudioModule(carType))
                {
                    var wheelsAudio = AudioDM3.transform.Find(CarPartNames.Audio.WHEELS_MODULE).gameObject;
                    var newWheelsAudio = Object.Instantiate(wheelsAudio, newAudioFab.transform);
                    var wheelAudioModule = newWheelsAudio.GetComponent<WheelsAudioModule>();
                    modularAudio.audioModules.Add(wheelAudioModule);

                    var carFab = carType.liveries[0].prefab;
                    var bogies = carFab.GetComponent<TrainCar>().Bogies;
                    var rearBogie = bogies.Last();
                    var frontBogie = bogies.First();

                    var wheelsFront = newWheelsAudio.transform.Find(CarPartNames.Audio.WHEELS_FRONT);
                    var wheelsRear = newWheelsAudio.transform.Find(CarPartNames.Audio.WHEELS_REAR);

                    wheelsFront.position = frontBogie.transform.position;
                    wheelsRear.position = rearBogie.transform.position;

                    // Set up slip audio
                    var pwm = carFab.GetComponentInChildren<PoweredWheelsManager>();
                    if (pwm)
                    {
                        var slipDefs = new WheelsAudioModule.WheelslipAudioDefinition[pwm.poweredWheels.Length];

                        for (int i = 0; i < slipDefs.Length; i++)
                        {
                            var layerLoc = GetCorrespondingWheelslipLayers(pwm.poweredWheels[i].transform, wheelsFront, wheelsRear);
                            var slipLayers = layerLoc.Find(CarPartNames.Audio.WHEELS_LAYERS_WHEELSLIP).GetComponent<LayeredAudio>();
                            var derailLayers = layerLoc.Find(CarPartNames.Audio.WHEELS_LAYERS_DAMAGED).GetComponent<LayeredAudio>();

                            slipDefs[i] = new WheelsAudioModule.WheelslipAudioDefinition()
                            {
                                wheelslipAudio = slipLayers,
                                derailedWheelslipAudio = derailLayers,
                                correspondingPoweredWheelsIndices = new[] { i }
                            };
                        }
                    }
                }

                carType.audioPrefab = newAudioFab;
            }
        }

        private static void ProcessAudioCopies(GameObject simAudioFab)
        {
            foreach (var item in simAudioFab.GetComponentsInChildren<CopyVanillaAudioSystem>())
            {
                var audio = Object.Instantiate(GetAudio(item.AudioSystem), item.transform);
                audio.transform.localPosition = Vector3.zero;
                audio.transform.localRotation = Quaternion.identity;

                item.InstancedObject = audio.gameObject;
                var readers = audio.GetComponents<LayeredAudioPortReader>();
                int length = Mathf.Min(readers.Length, item.Ports.Length);

                for (int i = 0; i < length; i++)
                {
                    readers[i].portId = item.Ports[i];
                }

                Object.Destroy(item);
            }
        }

        private static LayeredAudio GetAudio(VanillaAudioSystem audioSystem)
        {
            if (s_audioSystems.TryGetValue(audioSystem, out LayeredAudio audio))
            {
                return audio;
            }

            audio = audioSystem switch
            {
                #region Common

                VanillaAudioSystem.SandFlow => AudioS282.transform.Find("[sim] Engine/Sand/SandFlowLayers")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.TMOverspeed => AudioMicroshunter.transform.Find("[sim] Engine/TMOverspeed_Layered")
                    .GetComponent<LayeredAudio>(),

                #endregion

                #region Diesel

                #region DE2

                VanillaAudioSystem.DE2Engine => AudioDE2.transform.Find("[sim] Engine/Engine_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE2EnginePiston => AudioDE2.transform.Find("[sim] Engine/EnginePiston_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE2EngineIgnition => AudioDE2.transform.Find("[sim] Engine/EngineIgnition_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE2ElectricMotor => AudioDE2.transform.Find("[sim] Engine/ElectricMotor_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE2Horn => AudioDE2.transform.Find("[sim] Engine/Horn_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE2Compressor => AudioDE2.transform.Find("[sim] Engine/Compressor_Layered")
                    .GetComponent<LayeredAudio>(),

                #endregion

                #region DE6

                VanillaAudioSystem.DE6EngineIdle => AudioDE6.transform.Find("[sim] Engine/Engine_Idle")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE6EngineThrottling => AudioDE6.transform.Find("[sim] Engine/Engine_Throttling")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE6EngineIgnition => AudioDE6.transform.Find("[sim] Engine/EngineIgnition_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE6ElectricMotor => AudioDE6.transform.Find("[sim] Engine/ElectricMotor_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE6DynamicBrakeBlower => AudioDE6.transform.Find("[sim] Engine/DBBlower_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE6Horn => AudioDE6.transform.Find("[sim] Engine/LocoDiesel_Horn_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE6Bell => AudioDE6.transform.Find("[sim] Engine/Bell_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.DE6Compressor => AudioDE6.transform.Find("[sim] Engine/Compressor_Layered")
                    .GetComponent<LayeredAudio>(),

                #endregion

                #endregion

                #region Steam

                VanillaAudioSystem.SteamerCoalDump => AudioS282.transform.Find("[sim] Engine/CoalDump/SandFlowLayers")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerFire => AudioS282.transform.Find("[sim] Engine/Fire/Fire_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerFireboxWind => AudioS282.transform.Find("[sim] Engine/FireboxWind/WindFirebox_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerSafetyRelease => AudioS282.transform.Find("[sim] Engine/SteamSafetyRelease/SteamRelease_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerBlowdown => AudioS282.transform.Find("[sim] Engine/Blowdown/SteamRelease_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerChestAdmission => AudioS282.transform.Find("[sim] Engine/SteamChestAdmission/SteamChestAdmission_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerCylinderCrack => AudioS282.transform.Find("[sim] Engine/CylinderCrack/SteamRelease_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerInjector => AudioS282.transform.Find("[sim] Engine/Injector/WaterInFlow_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerValveGear => AudioS282.transform.Find("[sim] Engine/ValveGearMechanism/Mechanism_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerValveGearDamaged => AudioS282.transform.Find("[sim] Engine/ValveGearMechanism/DamagedMechanism_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerCrownSheet => AudioS282.transform.Find("[sim] Engine/CrownSheetBoiling/CrownSheetBoiling_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerAirPump => AudioS282.transform.Find("[sim] Engine/AirPump/AirPump_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerDynamo => AudioS282.transform.Find("[sim] Engine/Dynamo/Dynamo_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerBellRing => AudioS282.transform.Find("[sim] Engine/Bell_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.SteamerBellPump => AudioS282.transform.Find("[sim] Engine/Bell_Pump")
                    .GetComponent<LayeredAudio>(),

                #endregion

                #region Electric

                VanillaAudioSystem.BE2ElectricMotor => AudioMicroshunter.transform.Find("[sim] Engine/ElectricMotor_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.BE2TMController => AudioMicroshunter.transform.Find("[sim] Engine/TMController_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.BE2Compressor => AudioMicroshunter.transform.Find("[sim] Engine/Compressor_Layered")
                    .GetComponent<LayeredAudio>(),
                VanillaAudioSystem.BE2Horn => AudioMicroshunter.transform.Find("[sim] Engine/Horn_Layered")
                    .GetComponent<LayeredAudio>(),

                #endregion

                _ => throw new System.NotImplementedException(nameof(audio)),
            };

            s_audioSystems.Add(audioSystem, audio);
            return audio;
        }

        private static Transform GetCorrespondingWheelslipLayers(Transform wheelLoc, Transform layersFront, Transform layersRear)
        {
            const float DELTA = 0.01f;

            static bool Between(float t, float low, float high)
            {
                return (t > low) && (t < high);
            }

            float wheelZ = wheelLoc.position.z;
            if (Between(wheelZ, layersFront.position.z - DELTA, layersFront.position.z + DELTA))
            {
                return layersFront;
            }

            if (Between(wheelZ, layersRear.position.z - DELTA, layersRear.position.z + DELTA))
            {
                return layersRear;
            }

            var newPos = new Vector3(wheelZ, 0, 0);
            var newlayers = Object.Instantiate(layersFront.gameObject, layersFront.parent, true);
            newlayers.transform.position = newPos;

            return newlayers.transform;
        }
    }
}
