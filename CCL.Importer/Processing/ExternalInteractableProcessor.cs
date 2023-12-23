using CCL.Types;
using DV.CabControls;
using DV.Optimizers;
using DV.Simulation.Brake;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using System.ComponentModel.Composition;
using System.Linq;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    internal class ExternalInteractableProcessor : ModelProcessorStep
    {
        private static readonly GameObject _flatbedHandbrake;
        private static readonly GameObject _flatbedBrakeRelease;

        static ExternalInteractableProcessor()
        {
            var flatbedInteractables = TrainCarType.FlatbedEmpty.ToV2().externalInteractablesPrefab;
            _flatbedHandbrake = flatbedInteractables.transform.Find(CarPartNames.HANDBRAKE_SMALL).gameObject;
            _flatbedBrakeRelease = flatbedInteractables.transform.Find(CarPartNames.BRAKE_CYL_RELEASE).gameObject;
        }

        public override void ExecuteStep(ModelProcessor context)
        {
            if (CarTypes.IsRegularCar(context.Car))
            {
                var newFab = SetupFreightInteractables(context.Car.externalInteractablesPrefab);

                var brakeFeeders = newFab.AddComponent<HandbrakeFeedersController>();
                brakeFeeders.RefreshChildren();

                var keyboardCtrl = newFab.AddComponent<InteractablesKeyboardControl>();
                keyboardCtrl.RefreshChildren();

                var optimizer = newFab.AddComponent<PlayerOnCarScriptsOptimizer>();
                optimizer.scriptsToDisable = new MonoBehaviour[] { keyboardCtrl };

                context.Car.externalInteractablesPrefab = newFab;
            }
            else
            {
                var newFab = Object.Instantiate(context.Car.externalInteractablesPrefab, null);
                newFab.SetActive(false);
                Object.DontDestroyOnLoad(newFab);

                context.Car.externalInteractablesPrefab = newFab;
            }
        }

        private static GameObject SetupFreightInteractables(GameObject interactables)
        {
            GameObject newFab = Object.Instantiate(interactables, null);
            newFab.SetActive(false);
            Object.DontDestroyOnLoad(newFab);

            var existingChildren = Enumerable.Range(0, newFab.transform.childCount)
                .Select(i => newFab.transform.GetChild(i))
                .ToList();

            foreach (var current in existingChildren)
            {
                switch (current.name)
                {
                    case CarPartNames.DUMMY_HANDBRAKE_SMALL:
                        var newBrake = Object.Instantiate(_flatbedHandbrake, newFab.transform);
                        newBrake.transform.localPosition = current.localPosition;
                        newBrake.transform.localRotation = current.localRotation;
                        Object.Destroy(current.gameObject);
                        break;

                    case CarPartNames.DUMMY_BRAKE_RELEASE:
                        var newRelease = Object.Instantiate(_flatbedBrakeRelease, newFab.transform);
                        newRelease.transform.localPosition = current.localPosition;
                        newRelease.transform.localRotation = current.localRotation;
                        Object.Destroy(current.gameObject);
                        break;

                    default:
                        break;
                }
            }

            newFab.SetLayersRecursive(DVLayer.Interactable);
            FixControlColliders(newFab);

            return newFab;
        }

        private static void FixControlColliders(GameObject root)
        {
            var controls = root.GetComponentsInChildren<Collider>(true);
            foreach (var control in controls)
            {
                control.isTrigger = true;
            }
        }
    }
}
