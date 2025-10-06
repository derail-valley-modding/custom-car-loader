using CCL.Types;
using DV.CabControls;
using DV.Openables;
using DV.Optimizers;
using DV.Simulation.Brake;
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
        private static readonly GameObject _hopperHandbrake;
        private static readonly GameObject _s060Handbrake;
        private static readonly GameObject _s282Handbrake;
        private static readonly GameObject _de2Handbrake;
        private static readonly GameObject _dm3Handbrake;
        private static readonly GameObject _dh4Handbrake;
        private static readonly GameObject _microshunterHandbrake;
        private static readonly GameObject _dm1uHandbrake;

        private static readonly GameObject _de2FuelPort;
        private static readonly GameObject _be2ChargePort;

        static ExternalInteractableProcessor()
        {
            var flatbedInteractables = QuickAccess.Wagons.Flatbed.externalInteractablesPrefab;
            _flatbedHandbrake = flatbedInteractables.transform.Find(HANDBRAKE_SMALL).gameObject;
            _flatbedBrakeRelease = flatbedInteractables.transform.Find(BRAKE_CYL_RELEASE).gameObject;

            _hopperHandbrake = QuickAccess.Wagons.HopperBrown.externalInteractablesPrefab
                .transform.Find(HANDBRAKE_HOPPER).gameObject;

            _s060Handbrake = QuickAccess.Locomotives.S060.interiorPrefab
                .transform.Find(HANDBRAKE_S060).gameObject;
            _s282Handbrake = QuickAccess.Locomotives.S282B.externalInteractablesPrefab
                .transform.Find(HANDBRAKE_S282).gameObject;
            _de2Handbrake = QuickAccess.Locomotives.DE2.interiorPrefab
                .transform.Find(HANDBRAKE_DE2).gameObject;
            _dm3Handbrake = QuickAccess.Locomotives.DM3.interiorPrefab
                .transform.Find(HANDBRAKE_DM3).gameObject;
            _dh4Handbrake = QuickAccess.Locomotives.DH4.interiorPrefab
                .transform.Find(HANDBRAKE_DH4).gameObject;
            _microshunterHandbrake = QuickAccess.Locomotives.Microshunter.interiorPrefab
                .transform.Find(HANDBRAKE_MICROSHUNTER).gameObject;
            _dm1uHandbrake = QuickAccess.Locomotives.DM1U.interiorPrefab
                .transform.Find(HANDBRAKE_DM1U).gameObject;

            _de2FuelPort = QuickAccess.Locomotives.DE2.prefab
                .transform.Find($"{FUEL_CAP_ROOT}/{FUEL_CAP_DE2}").gameObject;
            _be2ChargePort = QuickAccess.Locomotives.Microshunter.prefab
                .transform.Find($"{CHARGE_PORT_ROOT}/{CHARGE_PORT_BE2}").gameObject;
        }

        public override void ExecuteStep(ModelProcessor context)
        {
            ProcessInteractablesIfPrefabExists(context.Car.externalInteractablesPrefab);
            ProcessInteractablesIfPrefabExists(context.Car.explodedExternalInteractablesPrefab);

            SetupExternalConnectors(context.Car.prefab);
        }

        private static void ProcessInteractablesIfPrefabExists(GameObject interactables)
        {
            if (interactables == null) return;

            SetupFreightInteractables(interactables);

            var brakeFeeders = interactables.AddComponent<HandbrakeFeedersController>();
            brakeFeeders.RefreshChildren();

            // Car has no handbrakes, delete controller.
            if (brakeFeeders.entries.Length == 0)
            {
                Object.Destroy(brakeFeeders);
            }

            var keyboardCtrl = interactables.AddComponent<InteractablesKeyboardControl>();
            keyboardCtrl.RefreshChildren();

            var optimizer = interactables.AddComponent<PlayerOnCarScriptsOptimizer>();
            optimizer.scriptsToDisable = new MonoBehaviour[] { keyboardCtrl };
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
                    case DUMMY_HANDBRAKE_LARGE:
                        Replace(interactables.transform, current, _hopperHandbrake);
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
                    case DUMMY_HANDBRAKE_DM1U:
                        Replace(interactables.transform, current, _dm1uHandbrake);
                        break;
                    default:
                        break;
                }
            }

            interactables.SetLayersRecursive(ModelProcessor.NonStandardLayerExclusion, DVLayer.Interactable);
            SetupOpenables(interactables);
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

        private static void SetupOpenables(GameObject prefab)
        {
            var children = prefab.GetComponentsInChildren<OpenableControl>();

            if (children.Length > 0)
            {
                prefab.AddComponent<DoorsAndWindowsController>().entries = children;
            }
        }
    }
}
