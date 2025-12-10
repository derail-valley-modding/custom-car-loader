using CCL.Creator.Utility;
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
        private const float SelectorWidth = 20.0f;
        private const float Separation = 2.0f;

        public static readonly GUIContent SelectorField = new GUIContent("    Quick Select",
            "You can quickly select a value from here instead of typing it manually\n" +
            "If this selector supports custom values, it will include an option for it");

        private int _selected = -1;

        private void OnGUIInternal(Rect position, SerializedProperty property, GUIContent label)
        {
            var att = (StringAndSelectorFieldAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);

            Rect controlPos1 = new Rect(position.x, position.y, position.width - SelectorWidth - Separation, EditorGUIUtility.singleLineHeight);
            Rect controlPos2 = new Rect(position.x + controlPos1.width + Separation, position.y, SelectorWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            EditorGUI.DelayedTextField(controlPos1, property);

            // Get the current string as an array index. If _selected hasn't been set yet, also run this code.
            if (EditorGUI.EndChangeCheck() || _selected < 0)
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
                    if (_selected < 0)
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

            using (new ResetIndentScope())
            {
                // Create the selector. Indent it a bit so it doesn't look like a property itself.
                var newIndex = EditorGUI.Popup(controlPos2, _selected, att.Options
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
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUIInternal(position, property, label);
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
