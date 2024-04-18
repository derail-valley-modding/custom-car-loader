using CCL.Types.Catalog.Diagram;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector.Catalog
{
    [CustomEditor(typeof(DiagramComponent), true)]
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

    [CustomEditor(typeof(Bogie), true)]
    internal class BogieEditor : Editor
    {
        private SerializedProperty _pivots;
        private SerializedProperty _wheels;

        private void OnEnable()
        {
            _pivots = serializedObject.FindProperty(nameof(Bogie.Pivots));
            _wheels = serializedObject.FindProperty(nameof(Bogie.Wheels));
        }

        public override void OnInspectorGUI()
        {
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
                Bogie.TryToAlignBogies(((Bogie)target).transform.root);
            }
        }
    }
}
