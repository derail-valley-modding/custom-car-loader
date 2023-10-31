using CCL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RandomPrefabPicker))]
    internal class RandomPrefabPickerEditor : UnityEditor.Editor
    {
        private SerializedProperty _chanceX;
        private SerializedProperty _chanceY;
        private SerializedProperty _chanceZ;

        private RandomPrefabPicker _picker;
        private bool _showPrefabs = false;
        private bool _showChances = false;

        private void OnEnable()
        {
            _chanceX = serializedObject.FindProperty("ChanceX");
            _chanceY = serializedObject.FindProperty("ChanceY");
            _chanceZ = serializedObject.FindProperty("ChanceZ");

            _picker = (RandomPrefabPicker)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Foldout with the prefabs and their weights.
            _showPrefabs = EditorGUILayout.Foldout(_showPrefabs, "Prefabs");

            if (_showPrefabs)
            {
                // Size selector.
                int length = Mathf.Max(0, EditorGUILayout.IntField("Size", _picker.Prefabs.Length));

                // Resize array if the length has changed or they're mismatched.
                if (length != _picker.Prefabs.Length || length != _picker.Weights.Length)
                {
                    _picker.ResizeArrays(length);
                }

                EditorGUI.indentLevel++;

                for (int i = 0; i < length; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    _picker.Prefabs[i] = (GameObject)EditorGUILayout.ObjectField(new GUIContent($"Prefab {i}",
                        "The prefab and its relative weight of being selected"), _picker.Prefabs[i], typeof(GameObject), false);
                    _picker.Weights[i] = EditorGUILayout.FloatField(_picker.Weights[i], GUILayout.MaxWidth(65));

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // Sliders for the chances to flip.
            _chanceX.floatValue = EditorGUILayout.Slider(new GUIContent("Chance to rotate around X",
                "The chance for the prefab to be rotated 180 degrees around the X axis"),
                _chanceX.floatValue, 0.0f, 1.0f);
            _chanceY.floatValue = EditorGUILayout.Slider(new GUIContent("Chance to rotate around Y",
                "The chance for the prefab to be rotated 180 degrees around the Y axis"),
                _chanceY.floatValue, 0.0f, 1.0f);
            _chanceZ.floatValue = EditorGUILayout.Slider(new GUIContent("Chance to rotate around Z",
                "The chance for the prefab to be rotated 180 degrees around the Z axis"),
                _chanceZ.floatValue, 0.0f, 1.0f);

            // Apply changes, everything below this is just display.
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            // Foldout with the weights converted to percentages.
            _showChances = EditorGUILayout.Foldout(_showChances, "Chances");

            if (_showChances)
            {
                EditorGUI.indentLevel++;

                float[] percents = _picker.GetPercentagesFromWeights();

                for (int i = 0; i < percents.Length; i++)
                {
                    EditorGUILayout.LabelField($"Prefab {i}", $"{percents[i] * 100.0f:F2}%");
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
        }
    }
}
