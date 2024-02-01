using CCL.Creator.Inspector;
using CCL.Creator.Utility;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using System;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    public class ControlWizard : EditorWindow
    {
        private static ControlWizard? _instance;

        [MenuItem("GameObject/CCL/Add Control", false, 10)]
        public static void ShowWindow(MenuCommand command)
        {
            _instance = GetWindow<ControlWizard>();
            _instance.Refresh((GameObject)command.context);
            _instance.titleContent = new GUIContent("CCL - Control Wizard");
            _instance.Show();
        }

        [MenuItem("GameObject/CCL/Add Control", true, 10)]
        public static bool OnContextMenuValidate()
        {
            return Selection.activeGameObject;
        }

        [Serializable]
        private class Settings : ScriptableObject
        {
            public GameObject TargetObject = null!;
            public string ControlName = "newControl";
            public DVControlClass ControlType;

            public bool IsFuseControl = false;

            [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL)]
            public string ControlledPortId;

            [FuseId]
            public string ControlledFuseId;
        }

        private Settings _settings;

        private void Refresh(GameObject target)
        {
            _settings = CreateInstance<Settings>();
            _settings.TargetObject = target;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");
            EditorStyles.label.wordWrap = true;

            var serialized = new SerializedObject(_settings);

            EditorGUILayout.PropertyField(serialized.FindProperty(nameof(Settings.ControlName)));
            EditorGUILayout.PropertyField(serialized.FindProperty(nameof(Settings.ControlType)));

            var fuseProp = serialized.FindProperty(nameof(Settings.IsFuseControl));
            EditorGUILayout.PropertyField(fuseProp);
            bool isFuse = fuseProp.boolValue;

            var idRect = EditorGUILayout.GetControlRect();
            if (isFuse)
            {
                FuseIdEditor.RenderProperty(
                    idRect,
                    serialized.FindProperty(nameof(Settings.ControlledFuseId)),
                    new GUIContent("Controlled Fuse"),
                    _settings.TargetObject.transform);
            }
            else
            {
                var filter = new PortIdAttribute(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL);

                PortIdEditor.RenderProperty(
                    idRect,
                    serialized.FindProperty(nameof(Settings.ControlledPortId)),
                    new GUIContent("Controlled Port"),
                    _settings.TargetObject.transform,
                    filter);
            }

            serialized.ApplyModifiedProperties();

            if (GUILayout.Button("Add Control"))
            {
                CreateControl(_settings);
                Close();
                return;
            }

            EditorGUILayout.EndVertical();
        }

        private static Type GetSpecType(DVControlClass type)
        {
            return type switch
            {
                DVControlClass.Lever => typeof(LeverProxy),
                DVControlClass.Button => typeof(ButtonProxy),
                DVControlClass.Rotary => typeof(RotaryProxy),
                DVControlClass.Puller => typeof(PullerProxy),
                DVControlClass.ToggleSwitch => typeof(ToggleSwitchProxy),
                DVControlClass.Wheel => typeof(WheelProxy),
                _ => throw new NotImplementedException(),
            };
        }

        private static void CreateControl(Settings settings)
        {
            var newControl = new GameObject(settings.ControlName);
            newControl.transform.SetParent(settings.TargetObject.transform, false);

            var specType = GetSpecType(settings.ControlType);
            newControl.AddComponent(specType);

            if (settings.IsFuseControl)
            {
                var feeder = newControl.AddComponent<InteractableFuseFeederProxy>();
                feeder.fuseId = settings.ControlledFuseId;
            }
            else
            {
                var feeder = newControl.AddComponent<InteractablePortFeederProxy>();
                feeder.portId = settings.ControlledPortId;
            }

            EditorUtility.SetDirty(settings.TargetObject);
            Selection.activeObject = newControl;
            EditorHelpers.SaveAndRefresh();

            DestroyImmediate(settings);
        }
    }
}
