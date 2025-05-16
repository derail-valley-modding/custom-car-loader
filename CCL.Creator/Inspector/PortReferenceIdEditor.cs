using CCL.Creator.Utility;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
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
            GUI.color = EditorHelpers.Colors.DEFAULT;

            EditorGUI.BeginProperty(position, label, property);

            IEnumerable<GameObject> sources = PortOptionHelper.GetAvailableSources((Component)component, false);

            var options = new List<PortOptionBase>
            {
                new PortReferenceOption(null, "Not Set")
            };
            options.AddRange(PortOptionHelper.GetPortReferenceOptions(sources));

            int selected = options.FindIndex(p => p.ID == currentValue);

            using (new GUIColorScope())
            {
                if (selected < 0 && !string.IsNullOrEmpty(currentValue))
                {
                    options.Add(new PortReferenceOption(currentValue));
                    selected = options.Count - 1;
                    GUI.color = EditorHelpers.Colors.DELETE_ACTION;
                }

                GUIContent[] optionNames = options.Select(p => new GUIContent(p.Description)).ToArray();

                int newIndex = EditorGUI.Popup(position, label, Math.Max(selected, 0), optionNames);

                if (newIndex != selected)
                {
                    property.stringValue = options[newIndex].ID;
                }
            }

            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}