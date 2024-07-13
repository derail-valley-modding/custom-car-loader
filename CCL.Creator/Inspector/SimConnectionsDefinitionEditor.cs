using CCL.Creator.Utility;
using CCL.Creator.Wizards;
using CCL.Types.Proxies.Ports;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(SimConnectionsDefinitionProxy))]
    internal class SimConnectionsDefinitionEditor : Editor
    {
        private SimConnectionsDefinitionProxy _proxy = null!;
        private SerializedProperty _connections = null!;
        private SerializedProperty _refConnections = null!;
        private ReorderableList _orderList = null!;

        private void OnEnable()
        {
            _connections = serializedObject.FindProperty(nameof(SimConnectionsDefinitionProxy.connections));
            _refConnections = serializedObject.FindProperty(nameof(SimConnectionsDefinitionProxy.portReferenceConnections));

            _orderList = EditorHelpers.CreateReorderableList(serializedObject, serializedObject.FindProperty(nameof(SimConnectionsDefinitionProxy.executionOrder)),
                true, true, true, "Execution Order");

            _orderList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = _orderList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(rect, element, GUIContent.none);
            };
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
            _orderList.DoLayoutList();
            EditorGUILayout.PropertyField(_connections);
            EditorGUILayout.PropertyField(_refConnections);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
