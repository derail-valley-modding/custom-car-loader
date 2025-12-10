using CCL.Creator.Utility;
using CCL.Types;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomPropertyDrawer(typeof(FuseIdAttribute))]
    public class FuseIdEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var fuseData = (FuseIdAttribute)attribute;
            var component = property.serializedObject.targetObject;

            RenderProperty(position, property, label, ((Component)component).transform, fuseData);
        }

        public static void RenderProperty(Rect position, SerializedProperty property, GUIContent label, Transform location, FuseIdAttribute fuseData)
        {
            string? currentValue = property.stringValue;
            if (string.IsNullOrEmpty(currentValue))
            {
                currentValue = null;
            }

            EditorGUI.BeginProperty(position, label, property);

            IEnumerable<GameObject> sources = PortOptionHelper.GetAllAvailableSources(location);

            var options = new List<PortOptionBase>
            {
                new FuseOption(null, "Not Set")
            };
            options.AddRange(PortOptionHelper.GetFuseOptions(sources));

            int selected = options.FindIndex(p => p.ID == currentValue);

            using (new GUIColorScope())
            {
                if (selected == 0)
                {
                    GUI.backgroundColor = fuseData.required ? EditorHelpers.Colors.DELETE_ACTION : EditorHelpers.Colors.WARNING;
                }
                else if (selected < 0 && !string.IsNullOrEmpty(currentValue))
                {
                    options.Add(new FuseOption(currentValue));
                    selected = options.Count - 1;
                    GUI.backgroundColor = EditorHelpers.Colors.DELETE_ACTION;
                }

                GUIContent[] optionNames = options.Select(p => new GUIContent(p.Description)).ToArray();

                int newIndex = EditorGUI.Popup(position, label, Math.Max(selected, 0), optionNames);

                if (newIndex != selected)
                {
                    property.stringValue = options[newIndex].ID;
                }
            }

            EditorGUI.EndProperty();
        }
    }
}