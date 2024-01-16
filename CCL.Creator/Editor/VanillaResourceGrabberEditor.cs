using CCL.Types.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Editor
{
    [CustomEditor(typeof(VanillaResourceGrabber<>))]
    internal class VanillaResourceGrabberEditor : UnityEditor.Editor
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

        private IVanillaResourceGrabber _grabber;
        private SerializedProperty _script = null!;
        private SerializedProperty _replacements = null!;

        private static bool s_showArray;
        private static bool s_showFields;

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
                EditorGUI.indentLevel++;

                int length = EditorGUILayout.DelayedIntField("Size", _replacements.arraySize);
                _replacements.arraySize = length;

                for (int i = 0; i < length; i++)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.LabelField($"Replacement {i}");
                    DrawReplacement(_replacements.GetArrayElementAtIndex(i), _grabber, fields);
                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
            }

            s_showFields = EditorGUILayout.Foldout(s_showFields, "Script fields");

            if (s_showFields)
            {
                EditorGUI.indentLevel++;

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

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawReplacement(SerializedProperty replacement, IVanillaResourceGrabber grabber, IEnumerable<AllowedFieldInfo> fields)
        {
            SerializedProperty nameI = replacement.FindPropertyRelative("NameIndex");
            SerializedProperty field = replacement.FindPropertyRelative("FieldName");
            SerializedProperty array = replacement.FindPropertyRelative("IsArray");
            SerializedProperty index = replacement.FindPropertyRelative("ArrayIndex");

            EditorGUI.indentLevel++;

            nameI.intValue = EditorGUILayout.Popup("Replacement Name", nameI.intValue, grabber.GetNames());
            EditorGUILayout.PropertyField(field);
            EditorGUILayout.PropertyField(array);
            GUI.enabled = array.boolValue;
            EditorGUILayout.PropertyField(index);
            GUI.enabled = true;

            if (!fields.Any(x => x.Name == field.stringValue))
            {
                EditorGUILayout.HelpBox($"Field '{field.stringValue}' does not exist!", MessageType.Error);
            }

            EditorGUI.indentLevel--;
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
                EditorGUILayout.TextField(f.IsArray ? "Array" : "Field", f.Name);
            }
        }



        [CustomEditor(typeof(SoundGrabber))]
        public class SoundGrabberEditor : VanillaResourceGrabberEditor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
            }
        }
    }
}
