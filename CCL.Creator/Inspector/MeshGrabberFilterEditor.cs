using CCL.Creator.Utility;
using CCL.Types.Components;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(MeshGrabberFilter))]
    internal class MeshGrabberFilterEditor : Editor
    {
        private MeshGrabberFilter _grabber = null!;
        private SerializedProperty _filter = null!;
        private SerializedProperty _name = null!;

        private void OnEnable()
        {
            _filter = serializedObject.FindProperty(nameof(MeshGrabberFilter.Filter));
            _name = serializedObject.FindProperty(nameof(MeshGrabberFilter.ReplacementName));
        }

        public override void OnInspectorGUI()
        {
            _grabber = (MeshGrabberFilter)target;

            EditorGUILayout.PropertyField(_filter);
            EditorHelpers.StringWithSearchField(_name, MeshGrabber.MeshNames, EditorGUIUtility.singleLineHeight * 4, 40);

            serializedObject.ApplyModifiedProperties();

            // Warn if the key doesn't exist.
            if (!MeshGrabber.MeshNames.Contains(_name.stringValue))
            {
                EditorGUILayout.HelpBox($"Name '{_name.stringValue}' does not exist!", MessageType.Error);
            }

            if (GUILayout.Button("Pick Filter In Object"))
            {
                _grabber.PickSame();
                AssetHelper.SaveAsset(_grabber);
            }

            // Quick replace when importing vanilla models directly and swapping before use.
            if (GUILayout.Button("Use Object Name"))
            {
                _grabber.ReplacementName = _grabber.gameObject.name;
                AssetHelper.SaveAsset(_grabber);
            }

            // Remove the mesh in the filter if it will be replaced anyways.
            GUI.enabled = _grabber.Filter;
            if (GUILayout.Button("Set Mesh In Filter To None"))
            {
                if (_grabber.Filter != null)
                {
                    _grabber.Filter.mesh = null;
                    AssetHelper.SaveAsset(_grabber);
                }
            }
            GUI.enabled = true;
        }
    }
}
