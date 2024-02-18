using CCL.Types.Components.HUD;
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
            _layout = (VanillaHUDLayout)target;

            EditorGUILayout.PropertyField(_type);

            if (_layout.HUDType != VanillaHUDLayout.BaseHUD.Custom)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Custom HUD layouts are currently a work in progress! Please use a vanilla layout for now.", MessageType.Error);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_custom);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Diesel HUDs", EditorStyles.boldLabel);

            if (GUILayout.Button("Set to Diesel-Electric"))
            {
                _layout.CustomHUDSettings.SetToDE();
            }

            if (GUILayout.Button("Set to Diesel-Hydraulic"))
            {
                _layout.CustomHUDSettings.SetToDH();
            }

            if (GUILayout.Button("Set to Diesel-Mechanical"))
            {
                _layout.CustomHUDSettings.SetToDM();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Electric HUDs", EditorStyles.boldLabel);

            if (GUILayout.Button("Set to Battery-Electric"))
            {
                _layout.CustomHUDSettings.SetToBE();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Steam HUDs", EditorStyles.boldLabel);

            if (GUILayout.Button("Set to Steam"))
            {
                _layout.CustomHUDSettings.SetToS();
            }
        }
    }
}
