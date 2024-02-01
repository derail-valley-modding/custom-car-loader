using CCL.Creator.Utility;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomPropertyDrawer(typeof(PortIdAttribute))]
    public class PortIdEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var portData = (PortIdAttribute)attribute;
            var component = property.serializedObject.targetObject;

            RenderProperty(position, property, label, ((Component)component).transform, portData);
        }

        public static void RenderProperty(Rect position, SerializedProperty property, GUIContent label, Transform location, PortIdAttribute portData)
        {
            string? currentValue = property.stringValue;
            if (string.IsNullOrEmpty(currentValue))
            {
                currentValue = null;
            }

            GUI.color = EditorHelpers.Colors.DEFAULT;

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            IEnumerable<GameObject> sources = PortOptionHelper.GetAvailableSources(location, portData.local);

            var options = new List<PortOptionBase>
            {
                new PortOption(null, "Not Set")
            };
            options.AddRange(PortOptionHelper.GetPortOptions(portData, sources));

            int selected = options.FindIndex(p => p.ID == currentValue);

            using (new GUIColorScope())
            {
                if (selected == 0)
                {
                    GUI.color = EditorHelpers.Colors.WARNING;
                }
                else if (selected < 0 && !string.IsNullOrEmpty(currentValue))
                {
                    options.Add(new PortOption(currentValue));
                    selected = options.Count - 1;
                    GUI.color = EditorHelpers.Colors.DELETE_ACTION;
                }

                string[] optionNames = options.Select(p => p.Description).ToArray();

                int newIndex = EditorGUI.Popup(position, Math.Max(selected, 0), optionNames);

                if (newIndex != selected)
                {
                    property.stringValue = options[newIndex].ID;
                }
            }

            EditorGUI.EndProperty();
        }
    }
}