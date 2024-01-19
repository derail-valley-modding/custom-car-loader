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
            var interactables = context.Car.externalInteractablesPrefab;

            if (CarTypes.IsRegularCar(context.Car))
            {
                SetupFreightInteractables(interactables);

                var brakeFeeders = interactables.AddComponent<HandbrakeFeedersController>();
                brakeFeeders.RefreshChildren();

                var keyboardCtrl = interactables.AddComponent<InteractablesKeyboardControl>();
                keyboardCtrl.RefreshChildren();

                var optimizer = interactables.AddComponent<PlayerOnCarScriptsOptimizer>();
                optimizer.scriptsToDisable = new MonoBehaviour[] { keyboardCtrl };
            }
        }

        private static void SetupFreightInteractables(GameObject interactables)
        {
            var existingChildren = Enumerable.Range(0, interactables.transform.childCount)
                .Select(i => interactables.transform.GetChild(i))
                .ToList();

            foreach (var current in existingChildren)
            {
                switch (current.name)
                {
                    case CarPartNames.DUMMY_HANDBRAKE_SMALL:
                        var newBrake = Object.Instantiate(_flatbedHandbrake, interactables.transform);
                        newBrake.transform.localPosition = current.localPosition;
                        newBrake.transform.localRotation = current.localRotation;
                        Object.Destroy(current.gameObject);
                        break;

                    case CarPartNames.DUMMY_BRAKE_RELEASE:
                        var newRelease = Object.Instantiate(_flatbedBrakeRelease, interactables.transform);
                        newRelease.transform.localPosition = current.localPosition;
                        newRelease.transform.localRotation = current.localRotation;
                        Object.Destroy(current.gameObject);
                        break;

                    default:
                        break;
                }
            }

            interactables.SetLayersRecursive(DVLayer.Interactable);
            FixControlColliders(interactables);
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
