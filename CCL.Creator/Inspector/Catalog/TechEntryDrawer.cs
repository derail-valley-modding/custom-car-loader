using CCL.Types.Catalog;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector.Catalog
{
    [CustomPropertyDrawer(typeof(TechEntry))]
    internal class TechEntryDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var icon = property.FindPropertyRelative(nameof(TechEntry.Icon));
            var desc = property.FindPropertyRelative(nameof(TechEntry.Description));
            var type = property.FindPropertyRelative(nameof(TechEntry.Type));

            EditorGUI.BeginProperty(position, GUIContent.none, property);

            Rect controlPos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var offset = new Vector2(0, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(controlPos, label);
            controlPos = EditorGUI.IndentedRect(controlPos);
            
            controlPos.position += offset;
            EditorGUI.PropertyField(controlPos, icon);
            controlPos.position += offset;
            EditorGUI.PropertyField(controlPos, desc);
            controlPos.position += offset;
            EditorGUI.PropertyField(controlPos, type);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 4;
        }
    }
}
