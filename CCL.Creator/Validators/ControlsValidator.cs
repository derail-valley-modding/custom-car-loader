using CCL.Types;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Weather;
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
                var hasCol = false;

                foreach (var go in control.colliderGameObjects)
                {
                    if (go == null)
                    {
                        result.Warning($"Control '{control.name}' has null entries in collider objects",
                            control, nameof(control.colliderGameObjects));
                        break;
                    }
                    
                    var colliders = go.GetComponentsInChildren<Collider>();

                    if (colliders.Length > 0)
                    {
                        hasCol = true;

                        foreach (var collider in colliders)
                        {
                            if (collider.isTrigger)
                            {
                                result.Warning($"Control '{control.name}'/ collider '{collider.name}' is set to trigger, but shouldn't",
                                    collider, nameof(collider.isTrigger));
                            }
                        }
                    }
                }

                if (!hasCol)
                {
                    result.Warning($"Control '{control.name}' does not have any colliders assigned, physical interaction will not work",
                        control, nameof(control.colliderGameObjects));
                }

                if (control.GetComponentsInChildren<MeshCollider>().Any(x => !x.convex))
                {
                    result.Fail($"Control '{control.name}' - non-convex mesh colliders are not supported in controls", control);
                }

                switch (control)
                {
                    case LeverProxy lever:
                        LimitWarning(lever.jointLimitMin, lever.jointLimitMax, "Lever");
                        break;
                    case RotaryProxy rotary:
                        LimitWarning(rotary.jointLimitMin, rotary.jointLimitMax, "Rotary");
                        break;
                    case ToggleSwitchProxy toggleSwitch:
                        LimitWarning(toggleSwitch.jointLimitMin, toggleSwitch.jointLimitMax, "Toggle Switch");
                        break;
                    case WheelProxy wheel:
                        LimitWarning(wheel.jointLimitMin, wheel.jointLimitMax, "Wheel");
                        break;
                    default:
                        break;
                }

                void LimitWarning(float min, float max, string name)
                {
                    if (min > max)
                    {
                        result.Warning($"{name} '{control.name}' limits bad setup: jointLimitMin must not be larger than jointLimitMax",
                            control, "jointLimitMin");
                    }
                }
            }

            foreach (var feeder in prefab.GetComponentsInChildren<InteractablePortFeederProxy>())
            {
                if (string.IsNullOrEmpty(feeder.portId))
                {
                    result.Warning($"Missing Port ID in InteractablePortFeeder '{feeder.name}'", feeder, nameof(feeder.portId));
                }
            }

            foreach (var kInput in prefab.GetComponentsInChildren<AKeyboardInputProxy>())
            {
                switch (kInput)
                {
                    case ButtonSetValueFromAxisInputProxy _:
                        if (!ComponentUtil.HasComponent<ButtonProxy>(kInput))
                        {
                            result.Fail($"ButtonSetValueFromAxisInputProxy lacks a ButtonProxy", kInput);
                        }
                        break;
                    case ButtonUseKeyboardInputProxy _:
                        if (!ComponentUtil.HasComponent<ButtonProxy>(kInput))
                        {
                            result.Fail($"ButtonUseKeyboardInputProxy lacks a ButtonProxy", kInput);
                        }
                        break;
                    case ToggleSwitchUseKeyboardInputProxy _:
                        if (!ComponentUtil.HasComponent<ToggleSwitchProxy>(kInput))
                        {
                            result.Fail($"ToggleSwitchUseKeyboardInputProxy lacks a ToggleSwitchProxy", kInput);
                        }
                        break;
                    default:
                        break;
                }
            }

            foreach (var openable in prefab.GetComponentsInChildren<OpenableControlProxy>())
            {
                if (!ComponentUtil.HasComponent<ControlSpecProxy>(openable))
                {
                    result.Fail($"OpenableControl lacks control", openable);
                }
            }

            return true;
        }
    }
}
