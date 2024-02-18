using CCL.Creator.Utility;
using CCL.Types;
using CCL.Types.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(VanillaResourceGrabber<>), true)]
    internal class VanillaResourceGrabberEditor : Editor
    {
        private struct AllowedFieldInfo
        {
            public string Name;
            public bool IsArray;

            public AllowedFieldInfo(string name, bool isArray)
            {
                Name = name;
                IsArray = isArray;
            }
        }

        // Keep these global to kinda mimic Unity behaviour.
        private static bool s_showArray;
        private static bool s_showFields;

        private IVanillaResourceGrabber _grabber = null!;
        private SerializedProperty _script = null!;
        private SerializedProperty _replacements = null!;

        private void OnEnable()
        {
            _script = serializedObject.FindProperty("ScriptToAffect");
            _replacements = serializedObject.FindProperty("Replacements");
        }

        public override void OnInspectorGUI()
        {
            _grabber = (IVanillaResourceGrabber)target;

            EditorGUILayout.PropertyField(_script);

            List<AllowedFieldInfo> fields = GetFieldInfosForScript(_grabber);

            s_showArray = EditorGUILayout.Foldout(s_showArray, "Replacements");

            if (s_showArray)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    // Array size changed *needs* to use a delayed field, or else the size would
                    // change every time the user typed or erased a number, messing up the array.
                    int length = EditorGUILayout.DelayedIntField("Size", _replacements.arraySize);
                    _replacements.arraySize = length;

                    for (int i = 0; i < length; i++)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        EditorGUILayout.LabelField($"Replacement {i}");
                        DrawReplacement(_replacements.GetArrayElementAtIndex(i), _grabber, fields);
                        EditorGUILayout.EndVertical();
                    }
                }
            }

            s_showFields = EditorGUILayout.Foldout(s_showFields, "Script fields");

            if (s_showFields)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    if (_grabber.GetScript())
                    {
                        if (fields.Count() == 0)
                        {
                            EditorGUILayout.HelpBox($"No supported fields ({_grabber.GetTypeOfResource().Name}) on script!", MessageType.Warning);
                        }

                        DrawFields(fields);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No script has been assigned!", MessageType.Warning);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawReplacement(SerializedProperty replacement, IVanillaResourceGrabber grabber, IEnumerable<AllowedFieldInfo> fields)
        {
            SerializedProperty rName = replacement.FindPropertyRelative(nameof(ResourceReplacement.ReplacementName));
            SerializedProperty field = replacement.FindPropertyRelative(nameof(ResourceReplacement.FieldName));
            SerializedProperty array = replacement.FindPropertyRelative(nameof(ResourceReplacement.IsArray));
            SerializedProperty index = replacement.FindPropertyRelative(nameof(ResourceReplacement.ArrayIndex));

            using (new EditorGUI.IndentLevelScope())
            {
                EditorHelpers.StringWithSearchField(rName, grabber.GetNames(), EditorGUIUtility.singleLineHeight * 4, 40);
                EditorHelpers.StringWithSearchField(field, fields.Select(x => x.Name), EditorGUIUtility.singleLineHeight * 4, 40, true);
                EditorGUILayout.PropertyField(array);
                GUI.enabled = array.boolValue;
                EditorGUILayout.PropertyField(index);
                GUI.enabled = true;

                // Warn if the key doesn't exist.
                if (!grabber.GetNames().Contains(rName.stringValue))
                {
                    EditorGUILayout.HelpBox($"Name '{rName.stringValue}' does not exist!", MessageType.Error);
                }

                // Warn if the field does not exist.
                if (!fields.Any(x => x.Name == field.stringValue))
                {
                    EditorGUILayout.HelpBox($"Field '{field.stringValue}' does not exist!", MessageType.Error);
                }
            }
        }

        private static List<AllowedFieldInfo> GetFieldInfosForScript(IVanillaResourceGrabber grabber)
        {
            if (grabber.GetScript() == null)
            {
                return new List<AllowedFieldInfo>();
            }

            Type t = grabber.GetScript().GetType();
            IEnumerable<FieldInfo> fields = t.GetRuntimeFields();
            List<AllowedFieldInfo> infos = new List<AllowedFieldInfo>();

            foreach (var f in fields)
            {
                // Find fields that support replacement, which are fields
                // of the type or arrays and lists of the type.
                if (f.FieldType == grabber.GetTypeOfResource())
                {
                    infos.Add(new AllowedFieldInfo(f.Name, false));
                }
                else if (grabber.GetTypeOfResourceIList().IsAssignableFrom(f.FieldType))
                {
                    infos.Add(new AllowedFieldInfo(f.Name, true));
                }
            }

            return infos;
        }

        private static void DrawFields(IEnumerable<AllowedFieldInfo> fields)
        {
            foreach (var f in fields)
            {
                // Why can't we have selectable labels with their own label?
                // Now it's a bit offset...
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(f.IsArray ? "Array" : "Field");
                EditorGUILayout.SelectableLabel(f.Name, GUI.skin.textField, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
