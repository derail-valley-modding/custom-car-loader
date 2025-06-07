using CCL.Creator.Utility;
using CCL.Types;
using CCL.Types.Proxies.Indicators;
using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(LabelLocalizer))]
    public class LabelLocalizerEditor : Editor
    {
        private SerializedProperty _key = null!;
        private SerializedProperty _modelType = null!;

        private void OnEnable()
        {
            _key = serializedObject.FindProperty(nameof(LabelLocalizer.key));
            _modelType = serializedObject.FindProperty(nameof(LabelLocalizer.ModelType));
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_key, new GUIContent("Key"));

            // Model Type
            var modelEnumValues = (LabelModelType[])Enum.GetValues(typeof(LabelModelType));
            var selectedModel = (LabelModelType)_modelType.intValue;
            int selectedModelIdx = Math.Max(Array.IndexOf(ModelOptions, selectedModel), 0);

            int newModelIndex = EditorGUILayout.Popup(new GUIContent("Model Type"), selectedModelIdx, ModelOptionNames);
            var newModel = ModelOptions[newModelIndex];
            _modelType.enumValueIndex = Array.IndexOf(modelEnumValues, newModel);

            if (!CarLabelKeyAttribute.CarLabelKeys.Contains(_key.stringValue) && !TranslationViewer.GetUserKeys().Contains(_key.stringValue))
            {
                EditorGUILayout.HelpBox("Custom key was not found in user translations, make sure to refresh web sources", MessageType.Warning);
            }

            if ((newModel != LabelModelType.None) && !newModel.HasFlag(LabelModelType.CustomText) && 
                ((MonoBehaviour)serializedObject.targetObject).GetComponent<TextMeshPro>())
            {
                EditorGUILayout.HelpBox("Selected model type already includes text, don't include your own TextMeshPro", MessageType.Error);
            }

            serializedObject.ApplyModifiedProperties();
        }

        public static readonly string[] ModelOptionNames = new[]
        {
            "None",
            "Offset, Built-In Text Mesh",
            "Offset, Custom Text Mesh",
            "Flush, Built-In Text Mesh",
            "Flush, Custom Text Mesh",
        };

        public static readonly LabelModelType[] ModelOptions = new[]
        {
            LabelModelType.None,
            LabelModelType.OffsetDefaultText,
            LabelModelType.OffsetCustomText,

            LabelModelType.FlushDefaultText,
            LabelModelType.FlushCustomText,
        };
    }
}