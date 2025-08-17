using CCL.Creator.Utility;
using CCL.Types.Components;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(MeshGrabberCollider))]
    internal class MeshGrabberColliderEditor : Editor
    {
        private MeshGrabberCollider _grabber = null!;
        private SerializedProperty _collider = null!;
        private SerializedProperty _name = null!;

        private void OnEnable()
        {
            _collider = serializedObject.FindProperty(nameof(MeshGrabberCollider.Collider));
            _name = serializedObject.FindProperty(nameof(MeshGrabberCollider.ReplacementName));
        }

        public override void OnInspectorGUI()
        {
            _grabber = (MeshGrabberCollider)target;

            EditorGUILayout.PropertyField(_collider);
            EditorHelpers.StringWithSearchField(_name, MeshGrabber.MeshNames, EditorGUIUtility.singleLineHeight * 4, 40);

            serializedObject.ApplyModifiedProperties();

            // Warn if the key doesn't exist.
            if (!MeshGrabber.MeshNames.Contains(_name.stringValue))
            {
                EditorGUILayout.HelpBox($"Name '{_name.stringValue}' does not exist!", MessageType.Error);
            }

            if (GUILayout.Button("Pick Collider In Object"))
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

            // Remove the mesh in the collider if it will be replaced anyways.
            using (new EditorGUI.DisabledScope(!_grabber.Collider))
            {
                if (GUILayout.Button("Set Mesh In Collider To None"))
                {
                    if (_grabber.Collider != null)
                    {
                        _grabber.Collider.sharedMesh = null;
                        AssetHelper.SaveAsset(_grabber);
                    }
                }
            }
        }
    }
}
