using CCL.Types.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Editor
{
    [CustomEditor(typeof(SoundGrabber))]
    internal class SoundGrabberEditor : UnityEditor.Editor
    {
        private SoundGrabber m_SoundGrabber;
        private SerializedProperty _script;
        private SerializedProperty _replacements;

        private static bool s_showArray;
        private static bool s_showFields;

        private void OnEnable()
        {
            _script = serializedObject.FindProperty("ScriptToAffect");
            _replacements = serializedObject.FindProperty("Replacements");
        }

        public override void OnInspectorGUI()
        {
            m_SoundGrabber = (SoundGrabber)target;

            EditorGUILayout.PropertyField(_script);
            //EditorGUILayout.PropertyField(_replacements);

            List<AudioFieldInfo> fields = GetFieldInfos(m_SoundGrabber.ScriptToAffect);

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
                    DrawReplacement(_replacements.GetArrayElementAtIndex(i), fields);
                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
            }

            s_showFields = EditorGUILayout.Foldout(s_showFields, "Script fields");

            if (s_showFields)
            {
                EditorGUI.indentLevel++;

                if (m_SoundGrabber.ScriptToAffect)
                {
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

        private static void DrawReplacement(SerializedProperty replacement, IEnumerable<AudioFieldInfo> fields)
        {
            SerializedProperty sound = replacement.FindPropertyRelative("SoundNameIndex");
            SerializedProperty field = replacement.FindPropertyRelative("FieldName");
            SerializedProperty array = replacement.FindPropertyRelative("IsArray");
            SerializedProperty index = replacement.FindPropertyRelative("Index");

            EditorGUI.indentLevel++;

            sound.intValue = EditorGUILayout.Popup("Sound Name", sound.intValue, SoundGrabber.SoundNames);
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

        private static void DrawFields(IEnumerable<AudioFieldInfo> fields)
        {
            foreach (AudioFieldInfo f in fields)
            {
                EditorGUILayout.TextField(f.IsArray ? "Array" : "Field", f.Name);
            }
        }

        private static List<AudioFieldInfo> GetFieldInfos(MonoBehaviour mb)
        {
            if (mb == null)
            {
                return new List<AudioFieldInfo>();
            }

            Type t = mb.GetType();
            FieldInfo[] fields = t.GetFields();
            List<AudioFieldInfo> infos = new List<AudioFieldInfo>();

            foreach (var f in fields)
            {
                if (f.FieldType == typeof(AudioClip))
                {
                    infos.Add(new AudioFieldInfo(f.Name, false));
                }
                else if (typeof(IEnumerable<AudioClip>).IsAssignableFrom(f.FieldType))
                {
                    infos.Add(new AudioFieldInfo(f.Name, true));
                }
            }

            return infos;
        }

        private struct AudioFieldInfo
        {
            public string Name;
            public bool IsArray;

            public AudioFieldInfo(string name, bool isArray)
            {
                Name = name;
                IsArray = isArray;
            }
        }
    }
}
