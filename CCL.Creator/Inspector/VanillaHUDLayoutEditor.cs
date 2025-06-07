using CCL.Types.HUD;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(VanillaHUDLayout))]
    internal class VanillaHUDLayoutEditor : Editor
    {
        private VanillaHUDLayout _layout = null!;
        private SerializedProperty _type = null!;
        private SerializedProperty _custom = null!;

        private void OnEnable()
        {
            _type = serializedObject.FindProperty(nameof(VanillaHUDLayout.HUDType));
            _custom = serializedObject.FindProperty(nameof(VanillaHUDLayout.CustomHUDSettings));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _layout = (VanillaHUDLayout)target;

            EditorGUILayout.PropertyField(_type);
            EditorGUILayout.Space();

            bool isCustom = _layout.HUDType == VanillaHUDLayout.BaseHUD.Custom;

            using (new EditorGUI.DisabledScope(!isCustom))
            {
                EditorGUILayout.Foldout(isCustom, "Custom Layout Settings");
            }

            if (!isCustom)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(_custom.FindPropertyRelative(nameof(CustomHUDLayout.Powertrain)));

                EditorGUILayout.PropertyField(_custom.FindPropertyRelative(nameof(CustomHUDLayout.BasicControls)));

                if (_layout.CustomHUDSettings.BasicControls.TMOrOilTemp == BasicControls.Slot1A.BothAlt)
                {
                    WarningBox("Displaying both Traction Motor Temperature and Oil Temperature is not supported correctly and so " +
                        "Oil Temperature will be moved to an empty slot.");
                }

                if (_layout.CustomHUDSettings.IsRPMTurbineVoltageAndPower())
                {
                    WarningBox("All options in slot 5 selected, this is not supported correctly and so " +
                        "Voltage will be moved to an empty slot.");
                }

                EditorGUILayout.PropertyField(_custom.FindPropertyRelative(nameof(CustomHUDLayout.Braking)));
                EditorGUILayout.PropertyField(_custom.FindPropertyRelative(nameof(CustomHUDLayout.Steam)));
                EditorGUILayout.PropertyField(_custom.FindPropertyRelative(nameof(CustomHUDLayout.Cab)));

                if (_layout.CustomHUDSettings.Cab.FuelDisplay == Cab.Slot21A.BothAlt)
                {
                    WarningBox("Displaying both fuel and battery is not supported correctly and so " +
                        "Battery will be moved to an empty slot.");
                }

                EditorGUILayout.PropertyField(_custom.FindPropertyRelative(nameof(CustomHUDLayout.Mechanical)));
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Diesel HUDs", EditorStyles.boldLabel);

            if (GUILayout.Button("Set to Diesel-Electric"))
            {
                Undo.RecordObject(_layout, "Set to Diesel-Electric");
                _layout.CustomHUDSettings.SetToDE();
            }

            if (GUILayout.Button("Set to Diesel-Hydraulic"))
            {
                Undo.RecordObject(_layout, "Set to Diesel-Hydraulic");
                _layout.CustomHUDSettings.SetToDH();
            }

            if (GUILayout.Button("Set to Diesel-Mechanical"))
            {
                Undo.RecordObject(_layout, "Set to Diesel-Mechanical");
                _layout.CustomHUDSettings.SetToDM();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Electric HUDs", EditorStyles.boldLabel);

            if (GUILayout.Button("Set to Battery-Electric"))
            {
                Undo.RecordObject(_layout, "Set to Battery-Electric");
                _layout.CustomHUDSettings.SetToBE();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Steam HUDs", EditorStyles.boldLabel);

            if (GUILayout.Button("Set to Steam"))
            {
                Undo.RecordObject(_layout, "Set to Steam");
                _layout.CustomHUDSettings.SetToS();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Quick brake setup", EditorStyles.boldLabel);

            if (GUILayout.Button("Self-lapping"))
            {
                Undo.RecordObject(_layout, "Self-lapping Brake Setup");
                _layout.CustomHUDSettings.SelfLappingBrakeSetup();
            }

            if (GUILayout.Button("Non Self-lapping"))
            {
                Undo.RecordObject(_layout, "Non Self-lapping Brake Setup");
                _layout.CustomHUDSettings.NonSelfLappingBrakeSetup();
            }
        }

        private static void WarningBox(string message)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(message, MessageType.Warning);
            EditorGUILayout.Space();
        }
    }
}
