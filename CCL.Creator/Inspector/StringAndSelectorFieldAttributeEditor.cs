using CCL.Types;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomPropertyDrawer(typeof(StringAndSelectorFieldAttribute), true)]
    internal class StringAndSelectorFieldAttributeEditor : PropertyDrawer
    {
        public static readonly GUIContent SelectorField = new GUIContent("    Quick Select",
            "You can quickly select a value from here instead of typing it manually\n" +
            "If this selector supports custom values, it will include an option for it");

        private int _selected = 0;

        private void OnGUIInternal(Rect position, SerializedProperty property, GUIContent label)
        {
            var att = (StringAndSelectorFieldAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);

            Rect controlPos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Vector2 offset = new Vector2(0, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            EditorGUI.DelayedTextField(controlPos, property);
            controlPos.position += offset;

            // Get the current string as an array index.
            if (EditorGUI.EndChangeCheck())
            {
                // Null or empty counts as Not Set.
                if (string.IsNullOrEmpty(property.stringValue))
                {
                    _selected = 0;
                }
                else
                {
                    _selected = att.Options.IndexOf(property.stringValue);

                    // If the string does not exist in the array...
                    if (_selected == -1)
                    {
                        // ...and the array supports custom strings, set it to custom.
                        // Otherwise set it to Not Set.
                        if (att.CustomAllowed)
                        {
                            _selected = att.Options.Count - 1;
                        }
                        else
                        {
                            _selected = 0;
                            property.stringValue = string.Empty;
                        }
                    }
                }
            }

            // Create the selector. Indent it a bit so it doesn't look like a property itself.
            var newIndex = EditorGUI.Popup(controlPos, SelectorField, _selected, att.Options
                .Select(x => new GUIContent(x)).ToArray());

            if (newIndex != _selected)
            {
                _selected = newIndex;

                // Clear the string on Not Set instead of writing... Not Set.
                if (newIndex == 0)
                {
                    property.stringValue = string.Empty;
                }
                else if (!(att.CustomAllowed && newIndex == att.Options.Count - 1))
                {
                    property.stringValue = att.Options[newIndex];
                }
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIInternal(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }

        // Easy adding of extra options to prevent duplicate code.
        public void OnGUIWithExtraOptions(Rect position, SerializedProperty property, GUIContent label, IEnumerable<string> extraOptions)
        {
            var att = (StringAndSelectorFieldAttribute)attribute;

            if (att.CustomAllowed)
            {
                var original = att.Options.ToList();

                att.Options.InsertRange(att.Options.Count - 1, extraOptions);

                OnGUIInternal(position, property, label);

                att.Options = original;
            }
            else
            {
                OnGUIInternal(position, property, label);
            }
        }
    }

    // These are needed to include the extra options from the settings.
    // If no settings options are needed, then the inspector above can
    // handle all cases.
    [CustomPropertyDrawer(typeof(CargoFieldAttribute), true)]
    internal class CargoFieldAttributeEditor : StringAndSelectorFieldAttributeEditor
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIWithExtraOptions(position, property, label, CCLEditorSettings.Settings.ExtraCargos);
        }
    }

    [CustomPropertyDrawer(typeof(GeneralLicenseFieldAttribute), true)]
    internal class GeneralLicenseFieldAttributeEditor : StringAndSelectorFieldAttributeEditor
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIWithExtraOptions(position, property, label, CCLEditorSettings.Settings.ExtraGeneralLicenses);
        }
    }

    [CustomPropertyDrawer(typeof(JobLicenseFieldAttribute), true)]
    internal class JobLicenseFieldAttributeEditor : StringAndSelectorFieldAttributeEditor
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIWithExtraOptions(position, property, label, CCLEditorSettings.Settings.ExtraJobLicenses);
        }
    }

    [CustomPropertyDrawer(typeof(AnyLicenseFieldAttribute), true)]
    internal class AnyLicenseFieldAttributeEditor : StringAndSelectorFieldAttributeEditor
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIWithExtraOptions(position, property, label,
                CCLEditorSettings.Settings.ExtraGeneralLicenses.Concat(CCLEditorSettings.Settings.ExtraJobLicenses));
        }
    }

    [CustomPropertyDrawer(typeof(PaintFieldAttribute), true)]
    internal class PaintFieldAttributeEditor : StringAndSelectorFieldAttributeEditor
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIWithExtraOptions(position, property, label, CCLEditorSettings.Settings.ExtraPaints);
        }
    }
}
