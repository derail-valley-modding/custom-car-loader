﻿using CCL.Importer.Processing;
using CCL.Importer.Types;
using CCL.Types;
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
        private static GameObject? s_audioDM3;
        private static GameObject AudioDM3 =>
            Extensions.GetCached(ref s_audioDM3, () => TrainCarType.LocoDM3.ToV2().parentType.audioPrefab);

        private static bool NeedsWheelsAudioModule(CCL_CarType carType)
        {
            var prefab = carType.liveries[0].prefab;
            var poweredWheels = prefab.GetComponentInChildren<PoweredWheelsManager>();
            var damage = prefab.GetComponent<DamageController>();

            return poweredWheels || (damage && (damage.wheels != null));
        }

        private static bool NeedsCustomAudio(CCL_CarType carType) =>
            (carType.SimAudioPrefab || NeedsWheelsAudioModule(carType));

        private static bool NeedsBrakeAirFix(CCL_CarType carType) =>
            carType.brakes.hasIndependentBrake || carType.brakes.trainBrake != TrainCarType_v2.BrakesSetup.TrainBrake.None;

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
                    var simAudioFab = carType.SimAudioPrefab;

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

                if (NeedsBrakeAirFix(carType))
                {
                    var brakesModule = newAudioFab.GetComponentInChildren<BrakesAudioModule>();
                    var dm3Brake = AudioDM3.transform.Find(CarPartNames.Audio.BRAKE_MODULE);

                    var cylinder = Object.Instantiate(dm3Brake.Find(CarPartNames.Audio.CYLINDER_EXHAUST).gameObject, brakesModule.transform);
                    var airflow = Object.Instantiate(dm3Brake.Find(CarPartNames.Audio.AIRFLOW).gameObject, brakesModule.transform);

                    brakesModule.brakeCylinderExhaustAudio = cylinder.GetComponent<LayeredAudio>();
                    brakesModule.airflowAudio = airflow.GetComponent<LayeredAudio>();
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
