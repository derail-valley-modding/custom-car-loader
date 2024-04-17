using CCL.Types.Catalog;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector.Catalog
{
    [CustomPropertyDrawer(typeof(LoadRating))]
    internal class LoadRatingDrawer : PropertyDrawer
    {
        private const int ExtraSpace = 3;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var tonnage = property.FindPropertyRelative(nameof(LoadRating.Tonnage));
            var color = property.FindPropertyRelative(nameof(LoadRating.Rating));
            var width = position.width;

            EditorGUI.BeginProperty(position, GUIContent.none, property);

            Rect controlPos = new Rect(position.x, position.y, width * (2 / 3f), EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(controlPos, tonnage, label);

            controlPos = new Rect(position.x + controlPos.width + ExtraSpace, position.y, width - controlPos.width - ExtraSpace, position.height);
            EditorGUI.PropertyField(controlPos, color, GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
