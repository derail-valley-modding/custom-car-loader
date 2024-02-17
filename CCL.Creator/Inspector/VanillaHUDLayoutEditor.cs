using CCL.Types.Components;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(VanillaHUDLayout))]
    internal class VanillaHUDLayoutEditor : Editor
    {
        private VanillaHUDLayout _layout = null!;

        private void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            _layout = (VanillaHUDLayout)target;

            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Diesel HUDs", EditorStyles.boldLabel);

            if (GUILayout.Button("Set to Diesel-Electric"))
            {
                _layout.SetToDE();
            }

            if (GUILayout.Button("Set to Diesel-Hydraulic"))
            {
                _layout.SetToDH();
            }

            if (GUILayout.Button("Set to Diesel-Mechanical"))
            {
                _layout.SetToDM();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Electric HUDs", EditorStyles.boldLabel);

            if (GUILayout.Button("Set to Battery-Electric"))
            {
                _layout.SetToBE();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Steam HUDs", EditorStyles.boldLabel);

            if (GUILayout.Button("Set to Steam"))
            {
                _layout.SetToS();
            }
        }
    }
}
