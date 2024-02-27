using CCL.Creator.Utility;
using CCL.Types.Components;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(SoundGrabberSource))]
    internal class SoundGrabberSourceEditor : Editor
    {
        private SoundGrabberSource _grabber = null!;
        private SerializedProperty _source = null!;
        private SerializedProperty _name = null!;

        private void OnEnable()
        {
            _source = serializedObject.FindProperty(nameof(SoundGrabberSource.Source));
            _name = serializedObject.FindProperty(nameof(SoundGrabberSource.ReplacementName));
        }

        public override void OnInspectorGUI()
        {
            _grabber = (SoundGrabberSource)target;

            EditorGUILayout.PropertyField(_source);
            EditorHelpers.StringWithSearchField(_name, SoundGrabber.SoundNames, EditorGUIUtility.singleLineHeight * 4, 40);

            serializedObject.ApplyModifiedProperties();

            // Warn if the key doesn't exist.
            if (!SoundGrabber.SoundNames.Contains(_name.stringValue))
            {
                EditorGUILayout.HelpBox($"Name '{_name.stringValue}' does not exist!", MessageType.Error);
            }

            if (GUILayout.Button("Pick Source In Object"))
            {
                _grabber.PickSame();
                AssetHelper.SaveAsset(_grabber);
            }
        }
    }
}
