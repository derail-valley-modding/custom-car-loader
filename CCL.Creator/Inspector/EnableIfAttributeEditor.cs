using CCL.Types;
using System.Reflection;
using System;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomPropertyDrawer(typeof(EnableIfAttribute), true)]
    internal class EnableIfAttributeEditor : PropertyDrawer
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private MethodInfo? _method;
        private FieldInfo? _field;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var att = (EnableIfAttribute)attribute;

            if (_method == null && _field == null)
            {
                SetupMethodOrField(property.serializedObject.targetObject.GetType(), att.Target);

                // If no target was found...
                if (_method == null && _field == null)
                {
                    Debug.LogError($"Could not find field, property or target {att.Target} in {property.serializedObject.targetObject.GetType().Name}");
                    base.OnGUI(position, property, label);
                    return;
                }
            }

            bool result = _method != null ?
                (bool)_method.Invoke(property.serializedObject.targetObject, null) :
                (bool)_field!.GetValue(property.serializedObject.targetObject);

            EditorGUI.BeginProperty(position, label, property);
            GUI.enabled = att.Invert ? !result : result;
            EditorGUI.PropertyField(position, property, true);
            GUI.enabled = true;
            EditorGUI.EndProperty();
            return;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private void SetupMethodOrField(Type t, string target)
        {
            // Try to get a method, if it exists, leave.
            _method = t.GetMethod(target, Flags);
            if (_method != null) return;

            // If no method exists, try for a property.
            var p = t.GetProperty(target, Flags);

            if (p != null)
            {
                _method = p.GetMethod;
                if (_method != null) return;
            }

            // At last, try for a field.
            _field = t.GetField(target, Flags);
        }
    }
}
