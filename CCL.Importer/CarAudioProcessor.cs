using CCL.Importer.Processing;
using DV.ModularAudioCar;
using DV.Simulation.Controllers;

namespace CCL.Importer
{
    public static class CarAudioProcessor
    {
        public static void InjectAudioPrefabs(TrainComponentPool pool)
        {
            foreach (var carType in CarManager.CustomCarTypes)
            {
                if (!carType.SimAudioPrefab || carType.audioPrefab) continue;

                carType.audioPoolSize = 10;

                var simAudioFab = ModelProcessor.CreateModifiablePrefab(carType.SimAudioPrefab);
                ModelProcessor.DoBasicProcessing(simAudioFab);

                var baseAudioFab = pool.defaultAudioPrefab;
                var newAudioFab = ModelProcessor.CreateModifiablePrefab(baseAudioFab);

                simAudioFab.transform.SetParent(newAudioFab.transform, false);
                simAudioFab.SetActive(true);

                // setup sim module
                var simAudio = simAudioFab.AddComponent<SimAudioModule>();
                simAudioFab.GetComponent<LayeredAudioSimReadersController>().OnValidate();
                simAudioFab.GetComponent<AudioClipSimReadersController>().OnValidate();

                // add to main audio controller
                var modularAudio = newAudioFab.GetComponent<CarModularAudio>();
                modularAudio.audioModules.Add(simAudio);

                carType.audioPrefab = newAudioFab;
                CCLPlugin.Log($"Created audio prefab for {carType.id}");
            }
        }
    }
}
