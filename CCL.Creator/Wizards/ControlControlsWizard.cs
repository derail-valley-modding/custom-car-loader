using CCL.Creator.Utility;
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
            BrakeRelease,
            
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
            Compressor,
            Dynamo,
            Lubricator,

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
            ToggleSwitch
        }

        private class KeyMap
        {
            public string Incremental;
            public string Absolute;
            public string Toggle;

            public bool HasAbsolute => !string.IsNullOrEmpty(Absolute);
            public bool HasToggle => !string.IsNullOrEmpty(Toggle);

            public KeyMap(string incremental) : this(incremental, string.Empty, string.Empty) { }

            public KeyMap(string incremental, string absolute) : this(incremental, absolute, string.Empty) { }

            public KeyMap(string incremental, string absolute, string toggle)
            {
                Incremental = incremental;
                Absolute = absolute;
                Toggle = toggle;
            }
        }

        private static Dictionary<ControlType, KeyMap> s_typeToKeyMap = new Dictionary<ControlType, KeyMap>()
        {
            { ControlType.Throttle, new KeyMap("ThrottleIncremental", "ThrottleAbsolute") },
            { ControlType.Reverser, new KeyMap("ReverserIncremental", "ReverserAbsolute") },

            { ControlType.TrainBrake, new KeyMap("BrakeIncremental", "BrakeAbsolute") },
            { ControlType.IndependentBrake, new KeyMap("IndependentBrakeIncremental", "IndependentBrakeAbsolute") },
            { ControlType.DynamicBrake, new KeyMap("DynamicBrakeIncremental", "DynamicBrakeAbsolute") },
            { ControlType.BrakeCutout, new KeyMap("BrakeCutoutIncremental", "BrakeCutoutAbsolute", "BrakeCutoutToggle") },
            { ControlType.Handbrake, new KeyMap("HandbrakeIncremental", "HandbrakeAbsolute", "HandbrakeToggle") },
            { ControlType.BrakeRelease, new KeyMap("ReleaseCyl", string.Empty, "ReleaseCyl") },

            { ControlType.GearboxA, new KeyMap("GearAIncrement", "GearAAbsolute") },
            { ControlType.GearboxB, new KeyMap("GearBIncrement", "GearBAbsolute") },

            { ControlType.HeadlightFront, new KeyMap("HeadlightFrontIncremental", "HeadlightFrontAbsolute") },
            { ControlType.HeadlightRear, new KeyMap("HeadlightRearIncremental", "HeadlightRearAbsolute") },
            { ControlType.CabLights, new KeyMap("CabLightIncremental", string.Empty, "CabLightToggle") },

            { ControlType.Sander, new KeyMap("SandIncremental", "SandAbsolute", "SandToggle") },
            { ControlType.Horn, new KeyMap("HornIncremental") },
            { ControlType.Bell, new KeyMap("BellIncremental", "BellAbsolute", "BellToggle") },
            { ControlType.Wipers, new KeyMap("WiperIncremental", "WiperAbsolute", "WiperToggle") },

            { ControlType.Injector, new KeyMap("InjectorIncremental", "InjectorAbsolute", "InjectorToggle") },
            { ControlType.Blowdown, new KeyMap("BlowdownIncremental", "BlowdownAbsolute", "BlowdownToggle") },
            { ControlType.Blower, new KeyMap("BlowerIncremental", "BlowerAbsolute", "BlowerToggle") },
            { ControlType.Damper, new KeyMap("DraftIncremental", "DraftAbsolute", "DraftToggle") },
            { ControlType.FireboxDoor, new KeyMap("FiredoorIncremental", "FiredoorAbsolute", "FiredoorToggle") },
            { ControlType.CylinderCocks, new KeyMap("CylCockIncremental", "CylCockAbsolute", "CylCockToggle") },
            { ControlType.Compressor, new KeyMap("AirPump", string.Empty, "AirPump") },
            { ControlType.Dynamo, new KeyMap("Dynamo", string.Empty, "Dynamo") },
            { ControlType.Lubricator, new KeyMap("Lubricator", string.Empty, "Lubricator") },

            { ControlType.Starter, new KeyMap("Starter", string.Empty, "Starter") },
            { ControlType.FuelCutoff, new KeyMap("FuelCutoff", string.Empty, "FuelCutoff") },
            { ControlType.StarterFuse, new KeyMap(string.Empty, string.Empty, "StarterFuse") },
            { ControlType.TractionMotorFuse, new KeyMap(string.Empty, string.Empty, "TractionMotorFuse") },
            { ControlType.ElectricsFuse, new KeyMap(string.Empty, string.Empty, "ElectricsFuse") },
        };

        private static ControlControlsWizard s_window = null!;

        private SerializedObject _serializedWindow = null!;
        [SerializeField, Tooltip("The control to set up")]
        private ControlSpecProxy _controlSpec = null!;
        [SerializeField, Tooltip("Automatically add a toggle feature if available")]
        private bool _autoAddToggle = true;
        [SerializeField, Tooltip("The control this spec belongs to")]
        private ControlType _controlType;
        [SerializeField, Tooltip("How inputs should behave")]
        private InputType _inputType;
        [SerializeField, Tooltip("If the input works in the opposite direction")]
        private bool _flip;

        [MenuItem("GameObject/CCL/Setup Control Controls", false, MenuOrdering.Cab.Control)]
        public static void ShowWindow(MenuCommand command)
        {
            var prev = s_window;
            s_window = GetWindow<ControlControlsWizard>();
            s_window._controlSpec = ((GameObject)command.context).GetComponent<ControlSpecProxy>();
            s_window.titleContent = new GUIContent("CCL - Control Controls Wizard");
            s_window.Show();

            if (s_window != prev)
            {
                s_window._serializedWindow = new SerializedObject(s_window);
            }

            s_window._serializedWindow.Update();
            s_window.Repaint();
        }

        [MenuItem("GameObject/CCL/Setup Control Controls", true, MenuOrdering.Cab.Control)]
        public static bool OnContextMenuValidate()
        {
            var go = Selection.activeGameObject;
            return go && go.GetComponent<ControlSpecProxy>();
        }

        private void OnGUI()
        {
            if (s_window == null)
            {
                s_window = GetWindow<ControlControlsWizard>();
            }

            if (s_window._serializedWindow == null)
            {
                s_window._serializedWindow = new SerializedObject(s_window);
            }

            EditorGUILayout.BeginVertical("box");

            using (new WordWrapScope(true))
            {
                EditorGUILayout.PropertyField(_serializedWindow.FindProperty(nameof(_controlSpec)));
                EditorHelpers.DrawSeparator();
                EditorGUILayout.PropertyField(_serializedWindow.FindProperty(nameof(_inputType)));
                EditorGUILayout.PropertyField(_serializedWindow.FindProperty(nameof(_controlType)));
                EditorGUILayout.PropertyField(_serializedWindow.FindProperty(nameof(_autoAddToggle)));
                EditorGUILayout.PropertyField(_serializedWindow.FindProperty(nameof(_flip)));
            }

            _serializedWindow.ApplyModifiedProperties();

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledGroupScope(!_controlSpec))
            {
                if (GUILayout.Button("Create"))
                {
                    AddInput(_controlSpec.gameObject, _controlType, _inputType, _autoAddToggle, _flip);
                }
            }

            EditorGUILayout.EndVertical();
        }

        private static void AddInput(GameObject go, ControlType control, InputType input, bool autoToggle, bool flip)
        {
            // If the control is missing from the dictionary, can't do anything else.
            if (!s_typeToKeyMap.TryGetValue(control, out var map))
            {
                Debug.LogError($"Could not find mapping for control type {control}");
                return;
            }

            // Pick a control type.
            switch (input)
            {
                case InputType.MouseScroll:
                    var mouse = go.AddComponent<MouseScrollKeyboardInputProxy>();
                    mouse.scrollAction = new ActionReference(map.Incremental, flip);
                    break;
                case InputType.ButtonUse:
                    if (!map.HasToggle)
                    {
                        Debug.LogError($"Control {control} has no toggle action for {input}, control was not added");
                        return;
                    }
                    var buttonUse = go.AddComponent<ButtonUseKeyboardInputProxy>();
                    buttonUse.useAction = new ActionReference(map.Toggle, flip);
                    break;
                case InputType.ButtonSetFromAxis:
                    var buttonSet = go.AddComponent<ButtonSetValueFromAxisInputProxy>();
                    buttonSet.useAction = new ActionReference(map.Incremental, flip);
                    break;
                case InputType.ToggleValue:
                    if (!map.HasToggle)
                    {
                        Debug.LogError($"Control {control} has no toggle action for {input}, control was not added");
                        return;
                    }
                    var toggleV = go.AddComponent<ToggleValueKeyboardInputProxy>();
                    toggleV.useAction = new ActionReference(map.Toggle, flip);
                    break;
                case InputType.ToggleSwitch:
                    if (!map.HasToggle)
                    {
                        Debug.LogError($"Control {control} has no toggle action for {input}, control was not added");
                        return;
                    }
                    var toggleS = go.AddComponent<ToggleSwitchUseKeyboardInputProxy>();
                    toggleS.useAction = new ActionReference(map.Toggle, flip);
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

            if (autoToggle && map.HasToggle && !IsToggleInput(input))
            {
                var toggle = go.AddComponent<ToggleValueKeyboardInputProxy>();
                toggle.useAction = new ActionReference(map.Toggle);
            }

            AssetHelper.SaveAsset(go);
        }

        private static bool IsToggleInput(InputType input) => input switch
        {
            InputType.ButtonUse => true,
            InputType.ToggleValue => true,
            InputType.ToggleSwitch => true,
            _ => false,
        };
    }
}
