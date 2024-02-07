using CCL.Importer.Processing;
using CCL.Importer.Types;
using DV.Damage;
using DV.ModularAudioCar;
using DV.Simulation.Controllers;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using LocoSim.Implementations.Wheels;
using System.Linq;
using UnityEngine;

namespace CCL.Importer
{
    public static class CarAudioProcessor
    {
        private static GameObject? _dm3AudioPrefab = null;
        private static GameObject DM3AudioPrefab =>
            Extensions.GetCached(ref _dm3AudioPrefab, () => TrainCarType.LocoDM3.ToV2().parentType.audioPrefab);

        private static bool NeedsWheelsAudioModule(CCL_CarType carType)
        {
            var prefab = carType.liveries[0].prefab;
            var poweredWheels = prefab.GetComponentInChildren<PoweredWheelsManager>();
            var damage = prefab.GetComponent<DamageController>();

            return poweredWheels || (damage && (damage.wheels != null));
        }

        private static bool NeedsCustomAudio(CCL_CarType carType) =>
            (carType.SimAudioPrefab || carType.RainAudioRoofOffset.HasValue || NeedsWheelsAudioModule(carType));

        public static void InjectAudioPrefabs(TrainComponentPool pool)
        {
            foreach (var carType in CarManager.CustomCarTypes)
            {
                if (carType.audioPrefab) return;

                if (!NeedsCustomAudio(carType)) continue;

                carType.audioPoolSize = 10;

                var simAudioFab = ModelProcessor.CreateModifiablePrefab(carType.SimAudioPrefab);
                ModelProcessor.DoBasicProcessing(simAudioFab);

                var newAudioFab = ModelProcessor.CreateModifiablePrefab(pool.defaultAudioPrefab);
                var modularAudio = newAudioFab.GetComponent<CarModularAudio>();

                if (carType.RainAudioRoofOffset.HasValue)
                {
                    var rainAudio = DM3AudioPrefab.transform.Find("RainModule").gameObject;
                    var newRainAudio = Object.Instantiate(rainAudio, newAudioFab.transform);
                    newRainAudio.transform.localPosition = carType.RainAudioRoofOffset.Value;

                    var rainAudioModule = newRainAudio.GetComponent<RainAudioModule>();
                    modularAudio.audioModules.Add(rainAudioModule);
                }

                if (carType.SimAudioPrefab)
                {
                    simAudioFab.transform.SetParent(newAudioFab.transform, false);
                    simAudioFab.SetActive(true);

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
                    var wheelsAudio = DM3AudioPrefab.transform.Find("WheelsModule").gameObject;
                    var newWheelsAudio = Object.Instantiate(wheelsAudio, newAudioFab.transform);
                    var wheelAudioModule = newWheelsAudio.GetComponent<WheelsAudioModule>();
                    modularAudio.audioModules.Add(wheelAudioModule);

                    var carFab = carType.liveries[0].prefab;
                    var bogies = carFab.GetComponent<TrainCar>().Bogies;
                    var rearBogie = bogies.Last();
                    var frontBogie = bogies.First();

                    var wheelsFront = newWheelsAudio.transform.Find("WheelsFront");
                    var wheelsRear = newWheelsAudio.transform.Find("WheelsRear");

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
                            var slipLayers = layerLoc.Find("WheelslipLayers").GetComponent<LayeredAudio>();
                            var derailLayers = layerLoc.Find("WheelDamagedLayers").GetComponent<LayeredAudio>();

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
