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
            var comps = livery.AllPrefabs.GetComponentsInChildren<AKeyboardInputProxy>(true);

            if (comps.Count == 0) return Skip();

            foreach (var input in comps)
            {
                switch (input)
                {
                    case AnalogSetValueJoystickInputProxy proxy:
                        if (CheckActionName(proxy.action))
                        {
                            result.Fail($"AnalogSetValueJoystickInputProxy {nameof(proxy.action)} is invalid",
                                input, nameof(proxy.action));
                        }
                        break;
                    case BinaryDecodeValueInputProxy proxy:
                        if (CheckActionName(proxy.action))
                        {
                            result.Fail($"BinaryDecodeValueInputProxy {nameof(proxy.action)} is invalid",
                                input, nameof(proxy.action));
                        }
                        break;
                    case ButtonUseKeyboardInputProxy proxy:
                        if (CheckActionName(proxy.useAction))
                        {
                            result.Fail($"ButtonUseKeyboardInputProxy {nameof(proxy.useAction)} is invalid",
                                input, nameof(proxy.useAction));
                        }
                        break;
                    case ButtonSetValueFromAxisInputProxy proxy:
                        if (CheckActionName(proxy.useAction))
                        {
                            result.Fail($"ButtonSetValueFromAxisInputProxy {nameof(proxy.useAction)} is invalid",
                                input, nameof(proxy.useAction));
                        }
                        break;
                    case FireboxKeyboardInputProxy proxy:
                        if (CheckActionName(proxy.lightFireAction))
                        {
                            result.Fail($"FireboxKeyboardInputProxy {nameof(proxy.lightFireAction)} is invalid",
                                input, nameof(proxy.lightFireAction));
                        }
                        if (CheckActionName(proxy.shovelCoalAction))
                        {
                            result.Fail($"FireboxKeyboardInputProxy {nameof(proxy.shovelCoalAction)} is invalid",
                                input, nameof(proxy.shovelCoalAction));
                        }
                        break;
                    case MouseScrollKeyboardInputProxy proxy:
                        if (CheckActionName(proxy.scrollAction))
                        {
                            result.Fail($"MouseScrollKeyboardInputProxy {nameof(proxy.scrollAction)} is invalid",
                                input, nameof(proxy.scrollAction));
                        }
                        break;
                    case PhysicsTorqueKeyboardInputProxy proxy:
                        if (CheckActionName(proxy.applyAction))
                        {
                            result.Fail($"PhysicsTorqueKeyboardInputProxy {nameof(proxy.applyAction)} is invalid",
                                input, nameof(proxy.applyAction));
                        }
                        break;
                    case PhysicsForceKeyboardInputProxy proxy:
                        if (CheckActionName(proxy.applyAction))
                        {
                            result.Fail($"PhysicsForceKeyboardInputProxy {nameof(proxy.applyAction)} is invalid",
                                input, nameof(proxy.applyAction));
                        }
                        break;
                    case ToggleSwitchUseKeyboardInputProxy proxy:
                        if (CheckActionName(proxy.useAction))
                        {
                            result.Fail($"ToggleSwitchUseKeyboardInputProxy {nameof(proxy.useAction)} is invalid",
                                input, nameof(proxy.useAction));
                        }
                        break;
                    case ToggleValueKeyboardInputProxy proxy:
                        if (CheckActionName(proxy.useAction))
                        {
                            result.Fail($"ToggleValueKeyboardInputProxy {nameof(proxy.useAction)} is invalid",
                                input, nameof(proxy.useAction));
                        }
                        break;
                    default:
                        break;
                }
            }

            return result;

            bool CheckActionName(ActionReference? action)
            {
                if (action == null) return false;

                return !hashes.Contains(action.name);
            }
        }
    }
}
