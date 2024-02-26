using CCL.Creator.Utility;
using CCL.Creator.Wizards;
using CCL.Types.Proxies.Ports;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(SimConnectionsDefinitionProxy))]
    internal class SimConnectionsDefinitionEditor : Editor
    {
        private SimConnectionsDefinitionProxy _proxy = null!;
        private SerializedProperty _order = null!;
        private SerializedProperty _connections = null!;
        private SerializedProperty _refConnections = null!;

        private void OnEnable()
        {
            _order = serializedObject.FindProperty(nameof(SimConnectionsDefinitionProxy.executionOrder));
            _connections = serializedObject.FindProperty(nameof(SimConnectionsDefinitionProxy.connections));
            _refConnections = serializedObject.FindProperty(nameof(SimConnectionsDefinitionProxy.portReferenceConnections));
        }

        public override void OnInspectorGUI()
        {
            _proxy = (SimConnectionsDefinitionProxy)target;

            EditorGUILayout.Foldout(true, "Actions");

            if (GUILayout.Button("Populate Components"))
            {
                _proxy.PopulateComponents();
                AssetHelper.SaveAsset(_proxy);
            }
            if (GUILayout.Button("Connection Wizard"))
            {
                SimulationEditorWindow.ShowWindow(_proxy);
            }

            EditorGUILayout.Space();
            EditorHelpers.ReorderableArrayField(_order);
            EditorGUILayout.PropertyField(_connections);
            EditorGUILayout.PropertyField(_refConnections);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
