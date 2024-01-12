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

        private bool _showArray;
        private bool _showFields;

        private void OnEnable()
        {
            _script = serializedObject.FindProperty("ScriptToAffect");
            _replacements = serializedObject.FindProperty("Replacements");
        }

        public override void OnInspectorGUI()
        {
            m_SoundGrabber = (SoundGrabber)target;

            EditorGUILayout.PropertyField(_script);
            EditorGUILayout.PropertyField(_replacements);

            List<AudioFieldInfo> fields = GetFieldInfos(m_SoundGrabber.ScriptToAffect);

            //_showArray = EditorGUILayout.Foldout(_showArray, "Replacements");

            //if (_showArray)
            //{
            //    EditorGUI.indentLevel++;

            //    int length = EditorGUILayout.DelayedIntField("Size", m_SoundGrabber.Replacements.Length);
            //    var old = m_SoundGrabber.Replacements.ToArray();
            //    m_SoundGrabber.Replacements = new SoundGrabber.SoundReplacement[length];

            //    for (int i = 0; i < length; i++)
            //    {
            //        var replace = new SoundGrabber.SoundReplacement();

            //        if (i < old.Length)
            //        {
            //            replace.SoundName = old[i].SoundName;
            //            replace.FieldName = old[i].FieldName;
            //            replace.IsArray = old[i].IsArray;
            //            replace.Index = old[i].Index;
            //        }
            //        else if (old.Length > 0)
            //        {
            //            var last = old.Last();
            //            replace.SoundName = last.SoundName;
            //            replace.FieldName = last.FieldName;
            //            replace.IsArray = last.IsArray;
            //            replace.Index = last.Index;
            //        }

            //        DrawReplacement(replace, i, fields);
            //        m_SoundGrabber.Replacements[i] = replace;
            //    }

            //    EditorGUI.indentLevel--;
            //}

            _showFields = EditorGUILayout.Foldout(_showFields, "Script fields");

            if (_showFields)
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

        private static void DrawReplacement(SoundGrabber.SoundReplacement replacement, int index, IEnumerable<AudioFieldInfo> fields)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField($"Replacement {index}");

            EditorGUI.indentLevel++;

            replacement.SoundName = EditorGUILayout.TextField("Sound Name", replacement.SoundName);
            replacement.FieldName = EditorGUILayout.TextField("Field Name", replacement.FieldName);
            replacement.IsArray = EditorGUILayout.Toggle("Is Array", replacement.IsArray);
            GUI.enabled = replacement.IsArray;
            replacement.Index = EditorGUILayout.IntField("Index", replacement.Index);
            GUI.enabled = true;

            if (!fields.Any(x => x.Name == replacement.FieldName))
            {
                EditorGUILayout.HelpBox($"Field '{replacement.FieldName}' does not exist!", MessageType.Error);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
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
