using CCL.Types.Catalog;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector.Catalog
{
    [CustomPropertyDrawer(typeof(CatalogScore))]
    internal class CatalogScoreDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var type = property.FindPropertyRelative(nameof(CatalogScore.ScoreType));
            var value = property.FindPropertyRelative(nameof(CatalogScore.Value));

            EditorGUI.BeginProperty(position, GUIContent.none, property);

            Rect controlPos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            GUI.enabled = (ScoreType)type.intValue != ScoreType.None;
            EditorGUI.PropertyField(controlPos, value, label);
            GUI.enabled = true;

            controlPos.position += new Vector2(0, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(controlPos, type);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}
