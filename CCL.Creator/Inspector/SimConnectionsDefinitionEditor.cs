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
        private SerializedProperty _autoClear = null!;
        private SerializedProperty _connections = null!;
        private SerializedProperty _refConnections = null!;
        private ReorderableList _orderList = null!;
        private ReorderableList _connectionList = null!;
        private ReorderableList _refConnectionList = null!;
        private bool _reorderMode = false;

        private void OnEnable()
        {
            _autoClear = serializedObject.FindProperty(nameof(SimConnectionsDefinitionProxy.AutoClearRemovedConnections));
            _connections = serializedObject.FindProperty(nameof(SimConnectionsDefinitionProxy.connections));
            _refConnections = serializedObject.FindProperty(nameof(SimConnectionsDefinitionProxy.portReferenceConnections));

            _orderList = EditorHelpers.CreateReorderableList(serializedObject, serializedObject.FindProperty(nameof(SimConnectionsDefinitionProxy.executionOrder)),
                true, true, true, "Execution Order");

            _orderList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = _orderList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(rect, element, GUIContent.none);
            };

            _connectionList = EditorHelpers.CreateReorderableList(serializedObject, _connections,
                true, true, true, "Port Connections");

            _connectionList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = _connectionList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.LabelField(rect, element.FindPropertyRelative(nameof(PortConnectionProxy.fullPortIdOut)).stringValue);
            };

            _refConnectionList = EditorHelpers.CreateReorderableList(serializedObject, _refConnections,
                true, true, true, "Port Reference Connections");

            _refConnectionList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = _refConnectionList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.LabelField(rect, element.FindPropertyRelative(nameof(PortReferenceConnectionProxy.portReferenceId)).stringValue);
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
            if (GUILayout.Button("Auto Sort From Hierarchy"))
            {
                _proxy.AutoSort();
                AssetHelper.SaveAsset(_proxy);
            }
            if (GUILayout.Button("Connection Wizard"))
            {
                SimulationEditorWindow.ShowWindow(_proxy);
            }

            EditorGUILayout.Space();
            _orderList.DoLayoutList();

            EditorGUILayout.PropertyField(_autoClear);
            _reorderMode = EditorGUILayout.Toggle("Reorder Mode", _reorderMode);

            if (_reorderMode)
            {
                _connectionList.DoLayoutList();
                _refConnectionList.DoLayoutList();
            }
            else
            {
                EditorGUILayout.PropertyField(_connections);
                EditorGUILayout.PropertyField(_refConnections);
            }

            if (GUILayout.Button("Auto Sort Connections"))
            {
                _proxy.AutoSortConnections();
                AssetHelper.SaveAsset(_proxy);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
