using CCL.Types;
using DV.CabControls;
using DV.Optimizers;
using DV.Simulation.Brake;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using System.ComponentModel.Composition;
using System.Linq;
using UnityEngine;
using static CCL.Types.CarPartNames.FuelPorts;
using static CCL.Types.CarPartNames.Interactables;

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

        private static readonly GameObject _de2FuelPort;
        private static readonly GameObject _be2ChargePort;

        static ExternalInteractableProcessor()
        {
            var flatbedInteractables = TrainCarType.FlatbedEmpty.ToV2().externalInteractablesPrefab;
            _flatbedHandbrake = flatbedInteractables.transform.Find(HANDBRAKE_SMALL).gameObject;
            _flatbedBrakeRelease = flatbedInteractables.transform.Find(BRAKE_CYL_RELEASE).gameObject;

            _s060Handbrake = TrainCarType.LocoS060.ToV2().interiorPrefab
                .transform.Find(HANDBRAKE_S060).gameObject;
            _s282Handbrake = TrainCarType.Tender.ToV2().externalInteractablesPrefab
                .transform.Find(HANDBRAKE_S282).gameObject;
            _de2Handbrake = TrainCarType.LocoShunter.ToV2().interiorPrefab
                .transform.Find(HANDBRAKE_DE2).gameObject;
            _dm3Handbrake = TrainCarType.LocoDM3.ToV2().interiorPrefab
                .transform.Find(HANDBRAKE_DM3).gameObject;
            _dh4Handbrake = TrainCarType.LocoDH4.ToV2().interiorPrefab
                .transform.Find(HANDBRAKE_DH4).gameObject;
            _microshunterHandbrake = TrainCarType.LocoMicroshunter.ToV2().interiorPrefab
                .transform.Find(HANDBRAKE_MICROSHUNTER).gameObject;

            _de2FuelPort = TrainCarType.LocoShunter.ToV2().prefab
                .transform.Find($"{FUEL_CAP_ROOT}/{FUEL_CAP_DE2}").gameObject;
            _be2ChargePort = TrainCarType.LocoMicroshunter.ToV2().prefab
                .transform.Find($"{CHARGE_PORT_ROOT}/{CHARGE_PORT_BE2}").gameObject;
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

            SetupExternalConnectors(context.Car.prefab);
        }

        private static void Replace(Transform parent, Transform current, GameObject newModel)
        {
            var newGo = Object.Instantiate(newModel, parent);
            newGo.transform.localPosition = current.localPosition;
            newGo.transform.localRotation = current.localRotation;
            Object.Destroy(current.gameObject);
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
                    case DUMMY_HANDBRAKE_SMALL:
                        Replace(interactables.transform, current, _flatbedHandbrake);
                        break;
                    case DUMMY_BRAKE_RELEASE:
                        Replace(interactables.transform, current, _flatbedBrakeRelease);
                        break;
                    case DUMMY_HANDBRAKE_S060:
                        Replace(interactables.transform, current, _s060Handbrake);
                        break;
                    case DUMMY_HANDBRAKE_S282:
                        Replace(interactables.transform, current, _s282Handbrake);
                        break;
                    case DUMMY_HANDBRAKE_DE2:
                        Replace(interactables.transform, current, _de2Handbrake);
                        break;
                    case DUMMY_HANDBRAKE_DM3:
                        Replace(interactables.transform, current, _dm3Handbrake);
                        break;
                    case DUMMY_HANDBRAKE_DH4:
                        Replace(interactables.transform, current, _dh4Handbrake);
                        break;
                    case DUMMY_HANDBRAKE_MICROSHUNTER:
                        Replace(interactables.transform, current, _microshunterHandbrake);
                        break;
                    default:
                        break;
                }
            }

            interactables.SetLayersRecursive(ModelProcessor.NonStandardLayerExclusion, DVLayer.Interactable);
            FixControlColliders(interactables);
        }

        private static void SetupExternalConnectors(GameObject prefab)
        {
            var existingChildren = Enumerable.Range(0, prefab.transform.childCount)
                .Select(i => prefab.transform.GetChild(i))
                .ToList();

            foreach (var current in existingChildren)
            {
                switch (current.name)
                {
                    case DUMMY_FUEL_CAP_DE2:
                        Replace(prefab.transform, current, _de2FuelPort);
                        break;

                    case DUMMY_CHARGE_PORT_BE2:
                        Replace(prefab.transform, current, _be2ChargePort);
                        break;
                }
            }
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
