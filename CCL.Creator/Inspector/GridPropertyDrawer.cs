using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    public abstract class GridPropertyDrawer<TProp> : PropertyDrawer
    {
        private const float PADDING_X = 1;
        private const float PADDING_Y = 1;

        protected SerializedProperty? Property { get; private set; }

        protected abstract int Columns { get; }
        protected abstract int Rows { get; }

        protected abstract void DrawCells(Rect bounds);

        protected Rect GetCellPosition(Rect bounds, int col, int row, int colSpan = 1, int rowSpan = 1)
        {
            return new Rect(
                bounds.x + bounds.width / Columns * col + PADDING_X,
                bounds.y + bounds.height / Rows * row + PADDING_Y,
                bounds.width * colSpan / Columns - 2 * PADDING_X,
                bounds.height * rowSpan / Rows - 2 * PADDING_Y
            );
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (base.GetPropertyHeight(property, label) + 2 * PADDING_Y) * Rows;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property == null) return;
            Property = property;

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var currentIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            DrawCells(position);

            EditorGUI.indentLevel = currentIndent;
            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
