using CCL.Creator.Utility;
using CCL.Types.Components;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(MaterialGrabberRenderer))]
    internal class MaterialGrabberRendererEditor : Editor
    {
        private static bool s_showArray;

        private MaterialGrabberRenderer _grabber = null!;
        private SerializedProperty _renderers = null!;
        private SerializedProperty _indices = null!;

        private void OnEnable()
        {
            _renderers = serializedObject.FindProperty(nameof(MaterialGrabberRenderer.RenderersToAffect));
            _indices = serializedObject.FindProperty(nameof(MaterialGrabberRenderer.Replacements));
        }

        public override void OnInspectorGUI()
        {
            _grabber = (MaterialGrabberRenderer)target;

            EditorGUILayout.PropertyField(_renderers);

            if (GUILayout.Button("Pick children"))
            {
                _grabber.PickChildren();
                AssetHelper.SaveAsset(_grabber);
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
            SerializedProperty renderer = replacement.FindPropertyRelative(nameof(MaterialGrabberRenderer.IndexToName.RendererIndex));
            SerializedProperty name = replacement.FindPropertyRelative(nameof(MaterialGrabberRenderer.IndexToName.ReplacementName));

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(renderer, new GUIContent("Material Index"));
                EditorHelpers.StringWithSearchField(name, MaterialGrabber.MaterialNames, EditorGUIUtility.singleLineHeight * 4, 40);

                // Warn if the key doesn't exist.
                if (!MaterialGrabber.MaterialNames.Contains(name.stringValue))
                {
                    EditorGUILayout.HelpBox($"Name '{name.stringValue}' does not exist!", MessageType.Error);
                }
            }
        }
    }
}
