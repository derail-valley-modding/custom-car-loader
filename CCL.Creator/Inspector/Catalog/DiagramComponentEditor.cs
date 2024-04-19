using CCL.Types.Catalog.Diagram;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector.Catalog
{
    [CustomEditor(typeof(DiagramComponent), true), CanEditMultipleObjects]
    internal class DiagramComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            FocusButton();
        }

        public static void FocusButton()
        {
            if (GUILayout.Button("Focus All"))
            {
                SceneView.lastActiveSceneView.Frame(GeometryUtility.CalculateBounds(DiagramComponent.FocusPoints, Matrix4x4.identity), false);
                SceneView.lastActiveSceneView.LookAt(Vector3.zero, Quaternion.LookRotation(Vector3.forward, Vector3.up));
            }
        }
    }

    [CustomEditor(typeof(BogieLayout), true)]
    internal class BogieLayoutEditor : Editor
    {
        private SerializedProperty _autoAlign = null!;
        private SerializedProperty _pivots = null!;
        private SerializedProperty _wheels = null!;

        private void OnEnable()
        {
            _autoAlign = serializedObject.FindProperty(nameof(BogieLayout.AutoAlign));
            _pivots = serializedObject.FindProperty(nameof(BogieLayout.Pivots));
            _wheels = serializedObject.FindProperty(nameof(BogieLayout.Wheels));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_autoAlign);
            EditorGUILayout.PropertyField(_pivots);

            int length = EditorGUILayout.DelayedIntField(new GUIContent("Number Of Wheels"), _wheels.arraySize);
            _wheels.arraySize = length;

            using (new EditorGUI.IndentLevelScope())
            {
                for (int i = 0; i < length; i++)
                {
                    EditorGUILayout.PropertyField(_wheels.GetArrayElementAtIndex(i), new GUIContent($"Wheel {i + 1} Powered"));
                }
            }

            serializedObject.ApplyModifiedProperties();

            DiagramComponentEditor.FocusButton();

            if (GUILayout.Button("Try To Align Bogies"))
            {
                BogieLayout.TryToAlignBogies(((BogieLayout)target).transform.root);
            }
        }
    }
}
