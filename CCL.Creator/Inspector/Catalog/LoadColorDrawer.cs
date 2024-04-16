using CCL.Types.Catalog;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector.Catalog
{
    [CustomPropertyDrawer(typeof(LoadColor))]
    internal class LoadColorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var tonnage = property.FindPropertyRelative(nameof(LoadColor.Tonnage));
            var color = property.FindPropertyRelative(nameof(LoadColor.Color));
            var width = position.width;

            EditorGUI.BeginProperty(position, GUIContent.none, property);

            Rect controlPos = new Rect(position.x, position.y, width * (2 / 3f), EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(controlPos, tonnage, label);

            controlPos = new Rect(position.x + controlPos.width, position.y, width - controlPos.width, position.height);
            EditorGUI.PropertyField(controlPos, color, GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
