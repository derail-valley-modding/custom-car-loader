using CCL.Creator.Utility;
using CCL.Types.Proxies.Indicators;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(LabelLocalizer))]
    public class LabelLocalizerEditor : Editor
    {
        private SerializedProperty _key;
        private SerializedProperty _selectedDefaultIdx;

        private void OnEnable()
        {
            _key = serializedObject.FindProperty(nameof(LabelLocalizer.key));
            _selectedDefaultIdx = serializedObject.FindProperty(nameof(LabelLocalizer.selectedDefaultIdx));
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_key);

            int selected = _selectedDefaultIdx.intValue;
            if (EditorGUI.EndChangeCheck() )
            {
                selected = Array.IndexOf(LabelLocalizer.DefaultOptions, _key.stringValue);
                selected = Math.Max(selected, 0);
                _selectedDefaultIdx.intValue = selected;
            }

            int newIndex = EditorGUILayout.Popup(new GUIContent("Default Keys:"), selected, LabelLocalizer.DefaultOptions);
            if (newIndex != selected)
            {
                _selectedDefaultIdx.intValue = newIndex;

                if (newIndex > 0)
                {
                    _key.stringValue = LabelLocalizer.DefaultOptions[newIndex];
                }
            }

            if ((newIndex == 0) && !TranslationViewer.GetUserKeys().Contains(_key.stringValue))
            {
                EditorGUILayout.HelpBox("Custom key was not found in user translations, make sure to refresh web sources", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}