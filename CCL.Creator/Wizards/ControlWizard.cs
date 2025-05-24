using CCL.Creator.Inspector;
using CCL.Creator.Utility;
using CCL.Types;
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

        [MenuItem("GameObject/CCL/Add Control", false, MenuOrdering.Cab.Control)]
        public static void ShowWindow(MenuCommand command)
        {
            _instance = GetWindow<ControlWizard>();
            _instance.Refresh((GameObject)command.context);
            _instance.titleContent = new GUIContent("CCL - Control Wizard");
            _instance.Show();
        }

        [MenuItem("GameObject/CCL/Add Control", true, MenuOrdering.Cab.Control)]
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

            public bool AddStaticInteractionArea = true;
            public bool IsFuseControl = false;
            public bool AutomaticReparenting = false;

            [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL)]
            public string ControlledPortId = string.Empty;

            [FuseId]
            public string ControlledFuseId = string.Empty;
        }

        private Settings _settings = null!;

        private void Refresh(GameObject target)
        {
            _settings = CreateInstance<Settings>();
            _settings.TargetObject = target;
        }

        private void OnGUI()
        {
            using (new WordWrapScope(true))
            {
                EditorGUILayout.BeginVertical("box");

                var serialized = new SerializedObject(_settings);

                EditorGUILayout.PropertyField(serialized.FindProperty(nameof(Settings.ControlName)));
                EditorGUILayout.PropertyField(serialized.FindProperty(nameof(Settings.ControlType)));
                EditorGUILayout.PropertyField(serialized.FindProperty(nameof(Settings.AddStaticInteractionArea)));

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

                EditorGUILayout.PropertyField(serialized.FindProperty(nameof(Settings.AutomaticReparenting)));

                serialized.ApplyModifiedProperties();
                EditorGUILayout.Space(18);

                if (GUILayout.Button("Add Control"))
                {
                    CreateControl(_settings);
                    Close();
                    return;
                }

                EditorGUILayout.EndVertical();
            }
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
            var parent = settings.TargetObject.transform.parent;
            var holder = new GameObject(settings.ControlName);

            // Control Spec
            var newControl = new GameObject($"C_{settings.ControlName}");
            newControl.transform.SetParent(holder.transform, false);

            var specType = GetSpecType(settings.ControlType);
            var spec = (ControlSpecProxy)newControl.AddComponent(specType);

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

            // Interaction Area
            if (settings.AddStaticInteractionArea)
            {
                var interactionArea = new GameObject($"IA_{settings.ControlName}");
                interactionArea.transform.SetParent(holder.transform.transform, false);
                interactionArea.SetActive(false);

                var staticIAProxy = interactionArea.AddComponent<StaticInteractionAreaProxy>();
                spec.nonVrStaticInteractionArea = staticIAProxy;

                var collider = interactionArea.AddComponent<SphereCollider>();
                collider.radius = 0.03f;
                collider.isTrigger = true;
            }

            if (parent != null && settings.AutomaticReparenting)
            {
                holder.transform.SetParent(parent, false);
                Utilities.CopyTransform(settings.TargetObject.transform, holder.transform);
                settings.TargetObject.transform.SetParent(newControl.transform);
                settings.TargetObject.transform.Reset();
            }
            else
            {
                holder.transform.SetParent(settings.TargetObject.transform, false);
            }

            EditorUtility.SetDirty(settings.TargetObject);
            Selection.activeObject = newControl;
            EditorHelpers.SaveAndRefresh();

            DestroyImmediate(settings);
        }
    }
}
