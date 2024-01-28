using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CCL.Creator.Editor
{
    [CustomPropertyDrawer(typeof(PortReferenceIdAttribute))]
    public class PortReferenceIdEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string? currentValue = property.stringValue;
            if (string.IsNullOrEmpty(currentValue))
            {
                currentValue = null;
            }

            var component = property.serializedObject.targetObject;
            
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            IEnumerable<GameObject> sources = PortOptionHelper.GetAvailableSources((Component)component, false);

            var options = new List<PortOptionBase>
            {
                new PortReferenceOption(null, "Not Set")
            };
            options.AddRange(PortOptionHelper.GetPortReferenceOptions(sources));

            int selected = options.FindIndex(p => p.ID == currentValue);

            if ((selected < 0) && !string.IsNullOrEmpty(currentValue))
            {
                options.Add(new PortReferenceOption(currentValue));
                selected = options.Count - 1;
            }

            string[] optionNames = options.Select(p => p.Description).ToArray();

            int newIndex = EditorGUI.Popup(position, Math.Max(selected, 0), optionNames);

            if (newIndex != selected)
            {
                property.stringValue = options[newIndex].ID;
            }

            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}