using CCL.Importer.Types;
using CCL.Types;
using DV;
using DV.ThingTypes;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL.Importer.Processing
{
    internal class ModelProcessor
    {
        private static readonly AggregateCatalog _catalog;

        static ModelProcessor()
        {
            _catalog = new AggregateCatalog();
            _catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
        }

        [ImportMany]
        private IEnumerable<IModelProcessorStep>? _steps = new IModelProcessorStep[0];

        public readonly List<IModelProcessorStep> SortedSteps;
        private readonly List<IModelProcessorStep> _completedSteps;

        public readonly CCL_CarVariant Car;
        public readonly TrainCarLivery BaseLivery;

        public ModelProcessor(CCL_CarVariant car)
        {
            Car = car;

            // Fetch the base type prefab for this car
            BaseLivery = Globals.G.Types.TrainCarType_to_v2[car.BaseCarType];

            var container = new CompositionContainer(_catalog, car);
            container.ComposeParts(this);

            SortedSteps = new List<IModelProcessorStep>(_steps);
            SortedSteps.Sort();

            _completedSteps = new();
        }

        public void ExecuteSteps()
        {
            CCLPlugin.Log($"Augmenting prefab for {Car.id}");

            CreateModifiablePrefabs();
            
            foreach (var prefab in Car.AllPrefabs)
            {
                HandleCustomSerialization(prefab);
            }

            foreach (var step in SortedSteps)
            {
                CCLPlugin.LogVerbose($"Execute step {step.GetType().Name}");
                step.ExecuteStep(this);
                _completedSteps.Add(step);
            }

            CCLPlugin.Log($"Finalized prefab for {Car.id}");
        }

        public T GetCompletedStep<T>()
            where T : IModelProcessorStep
        {
            return _completedSteps.OfType<T>().FirstOrDefault();
        }

        private void CreateModifiablePrefabs()
        {
            // Create a modifiable copy of the prefabs.
            Car.RemakePrefabs();

            // Create new TrainCar script.
            Car.prefab.name = Car.id;
            var newTrainCar = Car.prefab.AddComponent<TrainCar>();
            newTrainCar.carLivery = Car;

            // Set interior layers.
            if (Car.interiorPrefab)
            {
                ModelUtil.SetLayersRecursiveAndExclude(Car.interiorPrefab, DVLayer.Interactable, DVLayer.Train_Walkable);
            }
        }

        public static GameObject CreateModifiablePrefab(GameObject gameObject)
        {
            GameObject newFab = Object.Instantiate(gameObject, null);

            // Get enabled state of components on prefab.
            // Unity disables the attached components on a GameObject when
            // deactivating that object, and we don't want that or when we
            // instance this prefab again they will all be disabled.
            var states = newFab.GetComponents<MonoBehaviour>().ToDictionary(k => k, v => v.enabled);

            newFab.SetActive(false);
            Object.DontDestroyOnLoad(newFab);

            // Restore state.
            foreach (var state in states)
            {
                state.Key.enabled = state.Value;
            }

            return newFab;
        }

        public static void HandleCustomSerialization(GameObject prefab)
        {
            foreach (var component in prefab.GetComponentsInChildrenByInterface<ICustomSerialized>())
            {
                component.AfterImport();
            }
        }
    }
}
