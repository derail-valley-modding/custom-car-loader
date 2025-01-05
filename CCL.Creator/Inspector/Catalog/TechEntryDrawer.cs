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
            var offset = new Vector2(0, EditorGUIUtility.singleLineHeight + 2);

            EditorGUI.LabelField(controlPos, label);
            controlPos = EditorGUI.IndentedRect(controlPos);
            
            controlPos.position += offset;
            EditorGUI.PropertyField(controlPos, icon);

            GUI.enabled = (TechIcon)icon.intValue != TechIcon.None;

            controlPos.position += offset;
            controlPos.height = EditorGUI.GetPropertyHeight(desc);
            EditorGUI.PropertyField(controlPos, desc, true);
            offset.y = controlPos.height + 2;
            controlPos.position += offset;
            controlPos.height = EditorGUI.GetPropertyHeight(type);
            EditorGUI.PropertyField(controlPos, type);

            GUI.enabled = true;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var icon = property.FindPropertyRelative(nameof(TechEntry.Icon));
            var desc = property.FindPropertyRelative(nameof(TechEntry.Description));
            var type = property.FindPropertyRelative(nameof(TechEntry.Type));

            return EditorGUI.GetPropertyHeight(icon) + EditorGUI.GetPropertyHeight(desc) + EditorGUI.GetPropertyHeight(type)
                + EditorGUIUtility.singleLineHeight + 6;
        }
    }
}
