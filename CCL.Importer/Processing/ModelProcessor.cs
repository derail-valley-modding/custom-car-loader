using CCL.Importer.Types;
using CCL.Types;
using DV;
using DV.CabControls.Spec;
using DV.ThingTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL.Importer.Processing
{
    using UObject = UnityEngine.Object;

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
            // Create a modifiable copy of the prefab
            GameObject newFab = UObject.Instantiate(Car.prefab, null);
            newFab.SetActive(false);
            UObject.DontDestroyOnLoad(newFab);

            newFab.name = Car.id;
            Car.prefab = newFab;

            // Create new TrainCar script
            var newTrainCar = newFab.AddComponent<TrainCar>();
            newTrainCar.carLivery = Car;

            // create modifiable interior
            if (Car.interiorPrefab)
            {
                var newInterior = UObject.Instantiate(Car.interiorPrefab, null);
                newInterior.SetActive(false);
                UObject.DontDestroyOnLoad(newInterior);

                ModelUtil.SetLayersRecursiveAndExclude(newInterior, DVLayer.Interactable, DVLayer.Train_Walkable);

                Car.interiorPrefab = newInterior;
            }
        }

        private void HandleCustomSerialization(GameObject prefab)
        {
            foreach (var component in prefab.GetComponentsInChildrenByInterface<ICustomSerialized>())
            {
                component.AfterImport();
            }
        }
    }
}
