using CCL_GameScripts;
using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConfigurableBinding))]
public class ConfigurableBindingEditor : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty type = property.FindPropertyRelative(nameof(ConfigurableBinding.Type));
        var curType = (BindingType)Enum.GetValues(typeof(BindingType)).GetValue(type.enumValueIndex);

        int rows = (curType == BindingType.Constant) ? 1 : 3;
        return base.GetPropertyHeight(property, label) * rows;
    }

    private const int COLS = 2;
    private const int ROWS = 3;

    private Rect Cell(Rect bounds, int col, int row, int rows, int cols = COLS)
    {
        return new Rect(
            bounds.x + (bounds.width / cols) * col,
            bounds.y + (bounds.height / rows) * row,
            bounds.width / cols,
            bounds.height / rows
        );
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property == null) return;

        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // type spinner
        SerializedProperty type = property.FindPropertyRelative(nameof(ConfigurableBinding.Type));
        var curType = (BindingType)Enum.GetValues(typeof(BindingType)).GetValue(type.enumValueIndex);

        int rows = (curType == BindingType.Constant) ? 1 : 3;

        var typeRect = Cell(position, 0, 0, rows);
        EditorGUI.PropertyField(typeRect, type, GUIContent.none);

        //var bindingRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height / 2);
        var bindingRect = Cell(position, 1, 0, rows);

        switch (curType)
        {
            case BindingType.Constant:
                EditorGUI.PropertyField(bindingRect, property.FindPropertyRelative("ConstantValue"), GUIContent.none);
                break;

            case BindingType.SimOutput:
                EditorGUI.PropertyField(bindingRect, property.FindPropertyRelative("OutputBinding"), GUIContent.none);
                break;

            case BindingType.ControlInput:
                EditorGUI.PropertyField(bindingRect, property.FindPropertyRelative("ControlBinding"), GUIContent.none);
                break;
        }

        if (curType != BindingType.Constant)
        {
            var rect = Cell(position, 0, 1, rows, 3);
            EditorGUI.LabelField(rect, "Binding Min/Max:");
            rect = Cell(position, 1, 1, rows, 3);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("BoundMin"), GUIContent.none);
            rect = Cell(position, 2, 1, rows, 3);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("BoundMax"), GUIContent.none);

            rect = Cell(position, 0, 2, rows, 3);
            EditorGUI.LabelField(rect, "Local Min/Max:");
            rect = Cell(position, 1, 2, rows, 3);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("LocalMin"), GUIContent.none);
            rect = Cell(position, 2, 2, rows, 3);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("LocalMax"), GUIContent.none);
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
