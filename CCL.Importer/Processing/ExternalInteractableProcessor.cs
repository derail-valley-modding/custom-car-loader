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
    [RequiresStep(typeof(ProxyScriptProcessor))]
    internal class ExternalInteractableProcessor : ModelProcessorStep
    {
        private static readonly GameObject _flatbedHandbrake;
        private static readonly GameObject _flatbedBrakeRelease;
        private static readonly GameObject _s060Handbrake;
        private static readonly GameObject _s282Handbrake;
        private static readonly GameObject _de2Handbrake;
        private static readonly GameObject _dm3Handbrake;
        private static readonly GameObject _dh4Handbrake;
        private static readonly GameObject _microshunterHandbrake;

        static ExternalInteractableProcessor()
        {
            var flatbedInteractables = TrainCarType.FlatbedEmpty.ToV2().externalInteractablesPrefab;
            _flatbedHandbrake = flatbedInteractables.transform.Find(CarPartNames.HANDBRAKE_SMALL).gameObject;
            _flatbedBrakeRelease = flatbedInteractables.transform.Find(CarPartNames.BRAKE_CYL_RELEASE).gameObject;

            _s060Handbrake = TrainCarType.LocoS060.ToV2().interiorPrefab
                .transform.Find(CarPartNames.HANDBRAKE_S060).gameObject;
            _s282Handbrake = TrainCarType.Tender.ToV2().externalInteractablesPrefab
                .transform.Find(CarPartNames.HANDBRAKE_S282).gameObject;
            _de2Handbrake = TrainCarType.LocoShunter.ToV2().interiorPrefab
                .transform.Find(CarPartNames.HANDBRAKE_DE2).gameObject;
            _dm3Handbrake = TrainCarType.LocoDM3.ToV2().interiorPrefab
                .transform.Find(CarPartNames.HANDBRAKE_DM3).gameObject;
            _dh4Handbrake = TrainCarType.LocoDH4.ToV2().interiorPrefab
                .transform.Find(CarPartNames.HANDBRAKE_DH4).gameObject;
            _microshunterHandbrake = TrainCarType.LocoMicroshunter.ToV2().interiorPrefab
                .transform.Find(CarPartNames.HANDBRAKE_MICROSHUNTER).gameObject;
        }

        public override void ExecuteStep(ModelProcessor context)
        {
            var interactables = context.Car.externalInteractablesPrefab;

            if (interactables)
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
                void Replace(GameObject go)
                {
                    var newGo = Object.Instantiate(go, interactables.transform);
                    newGo.transform.localPosition = current.localPosition;
                    newGo.transform.localRotation = current.localRotation;
                    Object.Destroy(current.gameObject);
                }

                switch (current.name)
                {
                    case CarPartNames.DUMMY_HANDBRAKE_SMALL:
                        Replace(_flatbedHandbrake);
                        break;
                    case CarPartNames.DUMMY_BRAKE_RELEASE:
                        Replace(_flatbedBrakeRelease);
                        break;
                    case CarPartNames.DUMMY_HANDBRAKE_S060:
                        Replace(_s060Handbrake);
                        break;
                    case CarPartNames.DUMMY_HANDBRAKE_S282:
                        Replace(_s282Handbrake);
                        break;
                    case CarPartNames.DUMMY_HANDBRAKE_DE2:
                        Replace(_de2Handbrake);
                        break;
                    case CarPartNames.DUMMY_HANDBRAKE_DM3:
                        Replace(_dm3Handbrake);
                        break;
                    case CarPartNames.DUMMY_HANDBRAKE_DH4:
                        Replace(_dh4Handbrake);
                        break;
                    case CarPartNames.DUMMY_HANDBRAKE_MICROSHUNTER:
                        Replace(_microshunterHandbrake);
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
