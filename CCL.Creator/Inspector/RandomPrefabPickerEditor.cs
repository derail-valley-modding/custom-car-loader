using CCL.Types.Components;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(RandomPrefabPicker))]
    internal class RandomPrefabPickerEditor : Editor
    {
        private RandomPrefabPicker _picker = null!;
        private bool _showChances = false;

        private void OnEnable()
        {
            _picker = (RandomPrefabPicker)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            // Foldout with the weights converted to percentages.
            _showChances = EditorGUILayout.Foldout(_showChances, "Chances");

            if (_showChances)
            {
                EditorGUI.indentLevel++;

                float[] percents = _picker.GetPercentagesFromWeights();
                bool hasZero = false;

                for (int i = 0; i < percents.Length; i++)
                {
                    // Rounded to make it easier on the eyes.
                    EditorGUILayout.LabelField(_picker.Prefabs[i].Prefab.name, $"{percents[i] * 100.0f:F2}%");
                    hasZero |= _picker.Prefabs[i].Weight <= 0;
                }

                // Warn the user if this is intentional or not.
                if (hasZero)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("At least one choice has a weight of 0!", MessageType.Warning);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // Button to instantiate the prefab as if it was ingame, undoable.
            if (GUILayout.Button("Preview"))
            {
                GameObject go = _picker.InstantiateOne();

                if (go)
                {
                    Undo.RegisterCreatedObjectUndo(go, "Created preview");
                }
            }

            _picker.OnValidate();
        }
    }
}
