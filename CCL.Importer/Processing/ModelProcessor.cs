using CCL.Importer.Types;
using CCL.Types;
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
        private static Transform? s_holder;
        private static Transform Holder => Extensions.GetCached(ref s_holder, CreateHolder);

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

        public ModelProcessor(CCL_CarVariant car)
        {
            Car = car;

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

        private static Transform CreateHolder()
        {
            var go = new GameObject("[CCL HOLDER]");
            go.SetActive(false);
            Object.DontDestroyOnLoad(go);

            return go.transform;
        }

        public static GameObject CreateModifiablePrefab(GameObject gameObject)
        {
            GameObject newFab = Object.Instantiate(gameObject, Holder);

            // No (Clone), makes it look bad.
            newFab.name = gameObject.name;
            return newFab;
        }

        public static void HandleCustomSerialization(GameObject prefab)
        {
            foreach (var component in prefab.GetComponentsInChildrenByInterface<ICustomSerialized>())
            {
                component.AfterImport();
            }
        }

        public static void DoBasicProcessing(GameObject prefab)
        {
            CCLPlugin.LogVerbose($"Deserializing, processing grabbers and proxies for {prefab.name}");
            HandleCustomSerialization(prefab);
            GrabberProcessor.ProcessGrabbersOnPrefab(prefab);
            ShaderProcessor.ReplaceShaderGrabbers(prefab);
            ObjectInstancerProcessor.ProcessAll(prefab);
            MaterialProcessor.ProcessAll(prefab);
            Mapper.ProcessConfigs(prefab);
            Mapper.ClearComponentCache();
        }

        public static (bool, bool) NonStandardLayerExclusion(Transform t)
        {
            if (t.gameObject.TryGetComponent(out InteriorNonStandardLayer comp))
            {
                return (true, comp.includeChildren);
            }

            return (false, false);
        }
    }
}
