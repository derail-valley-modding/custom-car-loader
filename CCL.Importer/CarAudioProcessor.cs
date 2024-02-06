using CCL.Importer.Processing;
using DV.ModularAudioCar;
using DV.Simulation.Controllers;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using UnityEngine;

namespace CCL.Importer
{
    public static class CarAudioProcessor
    {
        public static void InjectAudioPrefabs(TrainComponentPool pool)
        {
            foreach (var carType in CarManager.CustomCarTypes)
            {
                if (carType.audioPrefab) return;

                if (!carType.SimAudioPrefab && !carType.AddRainAudioModule) continue;

                carType.audioPoolSize = 10;

                var simAudioFab = ModelProcessor.CreateModifiablePrefab(carType.SimAudioPrefab);
                ModelProcessor.DoBasicProcessing(simAudioFab);

                var newAudioFab = ModelProcessor.CreateModifiablePrefab(pool.defaultAudioPrefab);
                var modularAudio = newAudioFab.GetComponent<CarModularAudio>();

                if (carType.AddRainAudioModule)
                {
                    var dm3AudioPrefab = TrainCarType.LocoDM3.ToV2().parentType.audioPrefab;
                    var rainAudio = dm3AudioPrefab.transform.Find("RainModule").gameObject;
                    var newRainAudio = Object.Instantiate(rainAudio, newAudioFab.transform);
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

                    carType.audioPrefab = newAudioFab;
                    CCLPlugin.Log($"Created audio prefab for {carType.id}");
                }
            }
        }
    }
}
