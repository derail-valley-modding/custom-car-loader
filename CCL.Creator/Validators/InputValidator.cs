using CCL.Types;
using CCL.Types.Proxies.Controls;
using System.Linq;

using static CCL.Types.Proxies.Controls.AKeyboardInputProxy;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class InputValidator : LiveryValidator
    {
        public override string TestName => "Inputs";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            var result = Pass();
            var hashes = ControlActions.Names.ToHashSet();
            var tested = false;

            foreach (var input in livery.AllPrefabs.GetComponentsInChildren<AKeyboardInputProxy>(true))
            {
                switch (input)
                {
                    case AnalogSetValueJoystickInputProxy proxy:
                        if (CheckActionName(proxy.action))
                        {
                            result.Fail($"AnalogSetValueJoystickInputProxy {nameof(proxy.action)} is invalid", input);
                        }
                        break;
                    case BinaryDecodeValueInputProxy proxy:
                        if (CheckActionName(proxy.action))
                        {
                            result.Fail($"BinaryDecodeValueInputProxy {nameof(proxy.action)} is invalid", input);
                        }
                        break;
                    case ButtonUseKeyboardInputProxy proxy:
                        if (CheckActionName(proxy.useAction))
                        {
                            result.Fail($"ButtonUseKeyboardInputProxy {nameof(proxy.useAction)} is invalid", input);
                        }
                        break;
                    case ButtonSetValueFromAxisInputProxy proxy:
                        if (CheckActionName(proxy.useAction))
                        {
                            result.Fail($"ButtonSetValueFromAxisInputProxy {nameof(proxy.useAction)} is invalid", input);
                        }
                        break;
                    case FireboxKeyboardInputProxy proxy:
                        if (CheckActionName(proxy.lightFireAction))
                        {
                            result.Fail($"FireboxKeyboardInputProxy {nameof(proxy.lightFireAction)} is invalid", input);
                        }
                        if (CheckActionName(proxy.shovelCoalAction))
                        {
                            result.Fail($"FireboxKeyboardInputProxy {nameof(proxy.shovelCoalAction)} is invalid", input);
                        }
                        break;
                    case MouseScrollKeyboardInputProxy proxy:
                        if (CheckActionName(proxy.scrollAction))
                        {
                            result.Fail($"MouseScrollKeyboardInputProxy {nameof(proxy.scrollAction)} is invalid", input);
                        }
                        break;
                    case PhysicsTorqueKeyboardInputProxy proxy:
                        if (CheckActionName(proxy.applyAction))
                        {
                            result.Fail($"PhysicsTorqueKeyboardInputProxy {nameof(proxy.applyAction)} is invalid", input);
                        }
                        break;
                    case PhysicsForceKeyboardInputProxy proxy:
                        if (CheckActionName(proxy.applyAction))
                        {
                            result.Fail($"PhysicsForceKeyboardInputProxy {nameof(proxy.applyAction)} is invalid", input);
                        }
                        break;
                    case ToggleSwitchUseKeyboardInputProxy proxy:
                        if (CheckActionName(proxy.useAction))
                        {
                            result.Fail($"ToggleSwitchUseKeyboardInputProxy {nameof(proxy.useAction)} is invalid", input);
                        }
                        break;
                    case ToggleValueKeyboardInputProxy proxy:
                        if (CheckActionName(proxy.useAction))
                        {
                            result.Fail($"ToggleValueKeyboardInputProxy {nameof(proxy.useAction)} is invalid", input);
                        }
                        break;
                    default:
                        break;
                }

                tested = true;
            }

            return tested ? result : Skip();

            bool CheckActionName(ActionReference? action)
            {
                if (action == null) return false;

                return !hashes.Contains(action.name);
            }
        }
    }
}
