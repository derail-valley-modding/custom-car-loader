using CCL.Types;
using CCL.Types.Proxies.Controls;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using static CCL.Types.Proxies.Controls.AKeyboardInputProxy;

namespace CCL.Creator.Wizards
{
    internal class ControlControlsWizard : EditorWindow
    {
        private enum ControlType
        {
            Throttle,
            Reverser,

            TrainBrake,
            IndependentBrake,
            DynamicBrake,
            BrakeCutout,
            Handbrake,
            
            GearboxA,
            GearboxB,

            HeadlightFront,
            HeadlightRear,
            CabLights,

            Sander,
            Horn,
            Bell,
            Wipers,

            Injector,
            Blowdown,
            Blower,
            Damper,
            FireboxDoor,
            CylinderCocks,

            Starter,
            FuelCutoff,
            StarterFuse,
            TractionMotorFuse,
            ElectricsFuse
        }

        private enum InputType
        {
            MouseScroll,
            ButtonUse,
            ButtonSetFromAxis,
            ToggleValue,
            ToggleSwitch,
            BinaryDecode
        }

        private class KeyMap
        {
            public BaseKeyType Up;
            public BaseKeyType Down;
            public string Incremental;
            public string Absolute;
            public string Toggle;

            public bool HasAbsolute => !string.IsNullOrEmpty(Absolute);
            public bool HasToggle => !string.IsNullOrEmpty(Toggle);

            public KeyMap(BaseKeyType up, BaseKeyType down, string incremental) : this(up, down, incremental, string.Empty, string.Empty) { }

            public KeyMap(BaseKeyType up, BaseKeyType down, string incremental, string absolute) : this(up, down, incremental, absolute, string.Empty) { }

            public KeyMap(BaseKeyType up, BaseKeyType down, string incremental, string absolute, string toggle)
            {
                Up = up;
                Down = down;
                Incremental = incremental;
                Absolute = absolute;
                Toggle = toggle;
            }
        }

        private static Dictionary<ControlType, KeyMap> s_typeToKeyMap = new Dictionary<ControlType, KeyMap>()
        {
            { ControlType.Throttle, new KeyMap(BaseKeyType.IncreaseThrottle, BaseKeyType.DecreaseThrottle,
                "ThrottleIncremental", "ThrottleAbsolute") },
            { ControlType.Reverser, new KeyMap(BaseKeyType.IncreaseReverser, BaseKeyType.DecreaseReverser,
                "ReverserIncremental", "ReverserAbsolute") },

            { ControlType.TrainBrake, new KeyMap(BaseKeyType.IncreaseBrake, BaseKeyType.DecreaseBrake,
                "BrakeIncremental", "BrakeAbsolute") },
            { ControlType.IndependentBrake, new KeyMap(BaseKeyType.IncreaseIndependentBrake, BaseKeyType.DecreaseIndependentBrake,
                "IndependentBrakeIncremental", "IndependentBrakeAbsolute") },
            { ControlType.DynamicBrake, new KeyMap(BaseKeyType.IncreaseDynamicBrake, BaseKeyType.DecreaseDynamicBrake,
                "DynamicBrakeIncremental", "DynamicBrakeAbsolute") },
            { ControlType.BrakeCutout, new KeyMap(0, 0,
                "BrakeCutoutIncremental", "BrakeCutoutAbsolute", "BrakeCutoutToggle") },
            { ControlType.Handbrake, new KeyMap(BaseKeyType.IncreaseHandbrake, BaseKeyType.DecreaseHandbrake,
                "HandbrakeIncremental", "HandbrakeAbsolute", "HandbrakeToggle") },

            { ControlType.GearboxA, new KeyMap(BaseKeyType.IncreaseGearA, BaseKeyType.DecreaseGearA,
                "GearAIncrement", "GearAAbsolute") },
            { ControlType.GearboxB, new KeyMap(BaseKeyType.IncreaseGearB, BaseKeyType.DecreaseGearB,
                "GearBIncrement", "GearBAbsolute") },

            { ControlType.HeadlightFront, new KeyMap(BaseKeyType.IncreaseHeadlightFront, BaseKeyType.DecreaseHeadlightFront,
                "HeadlightFrontIncremental", "HeadlightFrontAbsolute") },
            { ControlType.HeadlightRear, new KeyMap(BaseKeyType.IncreaseHeadlightRear, BaseKeyType.DecreaseHeadlightRear,
                "HeadlightRearIncremental", "HeadlightRearAbsolute") },
            { ControlType.CabLights, new KeyMap(BaseKeyType.IncreaseCabLight, BaseKeyType.DecreaseCabLight,
                "CabLightIncremental", string.Empty, "CabLightToggle") },

            { ControlType.Sander, new KeyMap(BaseKeyType.IncreaseSand, BaseKeyType.DecreaseSand,
                "SandIncremental", "SandAbsolute", "SandToggle") },
            { ControlType.Horn, new KeyMap(BaseKeyType.IncreaseHorn, BaseKeyType.DecreaseHorn,
                "HornIncremental") },
            { ControlType.Bell, new KeyMap(BaseKeyType.Bell, 0,
                "BellIncremental", "BellAbsolute", "BellToggle") },
            { ControlType.Wipers, new KeyMap(BaseKeyType.IncreaseWiper, BaseKeyType.DecreaseWiper,
                "WiperIncremental", "WiperAbsolute", "WiperToggle") },

            { ControlType.Injector, new KeyMap(BaseKeyType.OpenInjector, BaseKeyType.CloseInjector,
                "InjectorIncremental", "InjectorAbsolute", "InjectorToggle") },
            { ControlType.Blowdown, new KeyMap(BaseKeyType.IncreaseWaterDump, BaseKeyType.DecreaseWaterDump,
                "BlowdownIncremental", "BlowdownAbsolute", "BlowdownToggle") },
            { ControlType.Blower, new KeyMap(BaseKeyType.IncreaseBlower, BaseKeyType.DecreaseBlower,
                "BlowerIncremental", "BlowerAbsolute", "BlowerToggle") },
            { ControlType.Damper, new KeyMap(BaseKeyType.IncreaseDraft, BaseKeyType.DecreaseDraft,
                "DraftIncremental", "DraftAbsolute", "DraftToggle") },
            { ControlType.FireboxDoor, new KeyMap(BaseKeyType.OpenFireDoor, BaseKeyType.CloseFireDoor,
                "FiredoorIncremental", "FiredoorAbsolute", "FiredoorToggle") },
            { ControlType.CylinderCocks, new KeyMap(BaseKeyType.DecreaseCylCock, BaseKeyType.IncreaseCylCock,
                "CylCockIncremental", "CylCockAbsolute", "CylCockToggle") },

            { ControlType.Starter, new KeyMap(BaseKeyType.Starter, 0,
                "Starter") },
            { ControlType.FuelCutoff, new KeyMap(BaseKeyType.FuelCutoff, 0,
                "FuelCutoff") },
            { ControlType.StarterFuse, new KeyMap(BaseKeyType.StarterFuse, 0,
                string.Empty, string.Empty, "StarterFuse") },
            { ControlType.TractionMotorFuse, new KeyMap(BaseKeyType.TractionMotorFuse, 0,
                string.Empty, string.Empty, "TractionMotorFuse") },
            { ControlType.ElectricsFuse, new KeyMap(BaseKeyType.ElectricsFuse, 0,
                string.Empty, string.Empty, "ElectricsFuse") },
        };

        private static ControlControlsWizard s_window = null!;

        private GameObject _target = null!;
        private bool _autoAddToggle = true;

        [MenuItem("GameObject/CCL/Setup Control Controls", false, MenuOrdering.Cab.Control)]
        public static void ShowWindow(MenuCommand command)
        {
            s_window = GetWindow<ControlControlsWizard>();
            s_window._target = (GameObject)command.context;
            s_window.titleContent = new GUIContent("CCL - Control Controls Wizard");
            s_window.Show();
        }

        [MenuItem("GameObject/CCL/Setup Control Controls", true, MenuOrdering.Cab.Control)]
        public static bool OnContextMenuValidate()
        {
            var go = Selection.activeGameObject;
            return go && go.GetComponent<ControlSpecProxy>();
        }

        private static void AddInput(GameObject go, ControlType control, InputType input, bool autoToggle)
        {
            if (!s_typeToKeyMap.TryGetValue(control, out var map))
            {
                Debug.LogError($"Could not find mapping for control type {control}");
                return;
            }
            switch (input)
            {
                case InputType.MouseScroll:
                    var mouse = go.AddComponent<MouseScrollKeyboardInputProxy>();
                    mouse.scrollUpKey = map.Up;
                    mouse.scrollDownKey = map.Down;
                    mouse.scrollAction = new ActionReference(map.Incremental);
                    break;
                case InputType.ButtonUse:
                    if (!map.HasToggle)
                    {
                        Debug.LogError($"Control {control} has no toggle action for {input}, control was not added");
                        return;
                    }
                    var buttonUse = go.AddComponent<ButtonUseKeyboardInputProxy>();
                    buttonUse.useKey = map.Up;
                    buttonUse.useAction = new ActionReference(map.Toggle);
                    break;
                case InputType.ButtonSetFromAxis:
                    var buttonSet = go.AddComponent<ButtonSetValueFromAxisInputProxy>();
                    buttonSet.useKey = map.Up;
                    buttonSet.useAction = new ActionReference(map.Incremental);
                    break;
                case InputType.ToggleValue:
                    if (!map.HasToggle)
                    {
                        Debug.LogError($"Control {control} has no toggle action for {input}, control was not added");
                        return;
                    }
                    var toggleV = go.AddComponent<ToggleValueKeyboardInputProxy>();
                    toggleV.toggleKey = map.Up;
                    toggleV.useAction = new ActionReference(map.Toggle);
                    break;
                case InputType.ToggleSwitch:
                    if (!map.HasToggle)
                    {
                        Debug.LogError($"Control {control} has no toggle action for {input}, control was not added");
                        return;
                    }
                    var toggleS = go.AddComponent<ToggleSwitchUseKeyboardInputProxy>();
                    toggleS.useKey = map.Up;
                    toggleS.useAction = new ActionReference(map.Toggle);
                    break;
                case InputType.BinaryDecode:
                    var binary = go.AddComponent<BinaryDecodeValueInputProxy>();
                    binary.action = new ActionReference(map.Incremental);
                    break;
                default:
                    Debug.LogError($"Unknown input type {input}");
                    return;
            }

            if (map.HasAbsolute)
            {
                var analog = go.GetComponent<AnalogSetValueJoystickInputProxy>();

                if (analog == null)
                {
                    analog = go.AddComponent<AnalogSetValueJoystickInputProxy>();
                }

                analog.action = new ActionReference(map.Absolute);
                analog.compressZeroToOne = control switch
                {
                    ControlType.HeadlightFront => true,
                    ControlType.HeadlightRear => true,
                    _ => false,
                };
            }

            if (autoToggle && map.HasToggle && input != InputType.ToggleValue && input != InputType.ToggleSwitch)
            {
                var toggle = go.AddComponent<ToggleValueKeyboardInputProxy>();
                toggle.toggleKey = 0;
                toggle.useAction = new ActionReference(map.Toggle);
            }
        }
    }
}
