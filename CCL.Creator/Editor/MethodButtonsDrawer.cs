using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CCL.Types;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator
{
    [CustomPropertyDrawer(typeof(RenderMethodButtonsAttribute))]
    public class MethodButtonsDrawer : PropertyDrawer
    {
        private readonly float buttonHeight = EditorGUIUtility.singleLineHeight * 1.2f;
        private List<ButtonInfo>? buttons = null;

        public override void OnGUI(Rect position, SerializedProperty targetProp, GUIContent label)
        {
            buttons ??= GetButtonsForField(targetProp).ToList();

            Rect foldoutRect = new Rect(position.x, position.y, position.width, 5 + buttonHeight);
            EditorGUI.Foldout(foldoutRect, true, "Actions", false);

            for (int i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i];
                Rect buttonRect = new Rect(position.x, position.y + ((i + 1) * (buttonHeight + 1)), position.width, buttonHeight - 1);

                if (GUI.Button(buttonRect, new GUIContent(button.Text, button.Tooltip ?? string.Empty)))
                {
                    button.Action.Invoke(null, new object[] { targetProp.serializedObject.targetObject });
                }
            }
        }

        private const BindingFlags ALL_INSTANCE = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags ALL_STATIC = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static IEnumerable<ButtonInfo> GetButtonsForField(SerializedProperty property)
        {
            var objectType = property.serializedObject.targetObject.GetType();
            var field = objectType.GetField(property.propertyPath, ALL_INSTANCE);

            foreach (var attr in field.GetCustomAttributes<MethodButtonAttribute>())
            {
                var action = GetMethod(attr.MethodName);

                if (action != null)
                {
                    yield return new ButtonInfo(attr.TextOverride!, action, attr.Tooltip);
                }
            }
        }

        private static MethodInfo? GetMethod(string methodName)
        {
            string[] parts = methodName.Split(':');
            if (parts.Length != 2) return null;

            if (GetType(parts[0]) is Type type)
            {
                if (type.GetMethod(parts[1], ALL_STATIC) is MethodInfo methodInfo)
                {
                    return methodInfo;
                }
            }
            return null;
        }

        private static Type? GetType(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        private static string SplitCamelCase(string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true) + buttonHeight * (buttons?.Count ?? 0 + 1);
        }

        private class ButtonInfo
        {
            public readonly string Text;
            public readonly MethodInfo Action;
            public readonly string? Tooltip;

            public ButtonInfo(string text, MethodInfo action, string? tooltip)
            {
                Text = text;
                Action = action;
                Tooltip = tooltip;
            }
        }
    }
}
