using CCL.Types;
using CCL.Types.Proxies.Controls;
using System.Linq;
using UnityEngine;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class ControlsValidator : LiveryValidator
    {
        public override string TestName => "Controls";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            var hasControls = false;
            var result = Pass();

            if (livery.interiorPrefab != null) hasControls |= CheckPrefab(livery.interiorPrefab, result);
            if (livery.explodedInteriorPrefab != null) hasControls |= CheckPrefab(livery.explodedInteriorPrefab, result);
            if (livery.externalInteractablesPrefab != null) hasControls |= CheckPrefab(livery.externalInteractablesPrefab, result);
            if (livery.explodedExternalInteractablesPrefab != null) hasControls |= CheckPrefab(livery.explodedExternalInteractablesPrefab, result);

            return hasControls ? result : Skip();
        }

        private static bool CheckPrefab(GameObject prefab, ValidationResult result)
        {
            var controls = prefab.GetComponentsInChildren<ControlSpecProxy>();

            if (controls.Length == 0) return false;

            foreach (var control in controls)
            {
                if (control.colliderGameObjects.Length == 0)
                {
                    result.Warning($"Control '{control.name}' does not have any colliders assigned, physical interaction will not work", control);
                }

                if (control.GetComponentsInChildren<MeshCollider>().Any(x => !x.convex))
                {
                    result.Fail($"Control '{control.name}' - non-convex mesh colliders are not supported in controls");
                }

                switch (control)
                {
                    case LeverProxy lever:
                        LimitWarning(control, lever.jointLimitMin, lever.jointLimitMax, "Lever");
                        break;
                    case RotaryProxy rotary:
                        LimitWarning(control, rotary.jointLimitMin, rotary.jointLimitMax, "Rotary");
                        break;
                    case ToggleSwitchProxy toggleSwitch:
                        LimitWarning(control, toggleSwitch.jointLimitMin, toggleSwitch.jointLimitMax, "Toggle Switch");
                        break;
                    case WheelProxy wheel:
                        LimitWarning(control, wheel.jointLimitMin, wheel.jointLimitMax, "Wheel");
                        break;
                    default:
                        break;
                }

                void LimitWarning(ControlSpecProxy control, float min, float max, string name)
                {
                    if (min > max)
                    {
                        result.Warning($"{name} '{control.name}' limits bad setup: jointLimitMin must not be larger than jointLimitMax", control);
                    }
                }
            }

            foreach (var feeder in prefab.GetComponentsInChildren<InteractablePortFeederProxy>())
            {
                if (string.IsNullOrEmpty(feeder.portId))
                {
                    result.Warning($"Missing Port ID in InteractablePortFeeder '{feeder.name}'", feeder);
                }
            }

            return true;
        }
    }
}
