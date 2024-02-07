using CCL.Types.Components;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(MaterialGrabberRenderer))]
    internal class MaterialGrabberRendererEditor : UnityEditor.Editor
    {
        private static bool s_showArray;

        private MaterialGrabberRenderer _grabber;
        private SerializedProperty _renderers;
        private SerializedProperty _indices;

        private void OnEnable()
        {
            _renderers = serializedObject.FindProperty(nameof(MaterialGrabberRenderer.RenderersToAffect));
            _indices = serializedObject.FindProperty(nameof(MaterialGrabberRenderer.Indeces));
        }

        public override void OnInspectorGUI()
        {
            _grabber = (MaterialGrabberRenderer)target;

            EditorGUILayout.PropertyField(_renderers);

            if (GUILayout.Button("Pick children"))
            {
                _grabber.PickChildren();
            }

            EditorGUILayout.Space();

            s_showArray = EditorGUILayout.Foldout(s_showArray, "Replacements");

            if (s_showArray)
            {
                int maxLength = int.MaxValue;

                foreach (var renderer in _grabber.RenderersToAffect)
                {
                    if (renderer)
                    {
                        maxLength = Mathf.Min(maxLength, renderer.sharedMaterials.Length);
                    }
                }

                using (new EditorGUI.IndentLevelScope())
                {
                    int length = EditorGUILayout.DelayedIntField("Size", _indices.arraySize);
                    _indices.arraySize = length;

                    for (int i = 0; i < length; i++)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        EditorGUILayout.LabelField($"Replacement {i}");
                        DrawReplacement(_indices.GetArrayElementAtIndex(i), maxLength);
                        EditorGUILayout.EndVertical();
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawReplacement(SerializedProperty replacement, int maxLength)
        {
            SerializedProperty renderer = replacement.FindPropertyRelative("RendererIndex");
            SerializedProperty name = replacement.FindPropertyRelative("NameIndex");

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(renderer, new GUIContent("Material Index"));
                name.intValue = EditorGUILayout.Popup("Replacement Name", name.intValue, MaterialGrabber.MaterialNames);

                // Warn if length is outside array bounds.
                if (renderer.intValue >= maxLength)
                {
                    EditorGUILayout.HelpBox($"Material index is outside array bounds (max {maxLength})!", MessageType.Error);
                }
            }
        }
    }
}
