using CCL.Types.Catalog;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector.Catalog
{
    [CustomPropertyDrawer(typeof(TechDescription))]
    internal class TechDescriptionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var entries = property.FindPropertyRelative(nameof(TechDescription.Entries));

            EditorGUI.BeginProperty(position, GUIContent.none, property);

            Rect controlPos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var offset = new Vector2(0, EditorGUIUtility.singleLineHeight + 2);

            property.isExpanded = EditorGUI.Foldout(controlPos, property.isExpanded, label);

            if (property.isExpanded)
            {
                controlPos = EditorGUI.IndentedRect(controlPos);

                controlPos.position += offset;
                entries.arraySize = EditorGUI.DelayedIntField(controlPos, "Sequence Size", entries.arraySize);

                for (int i = 0; i < entries.arraySize; i++)
                {
                    controlPos.position += offset;
                    EditorGUI.PropertyField(controlPos, entries.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(TechDescription.Entry.Key)));
                    controlPos.position += offset;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var entries = property.FindPropertyRelative(nameof(TechDescription.Entries));
            int count = property.isExpanded ? entries.arraySize : 0;
            int extra = property.isExpanded ? 2 : 1;
            return EditorGUIUtility.singleLineHeight *
                // Number of entries + size field.
                ((2 * count) + extra) +
                // Padding.
                ((count + (extra * 2)) * 2);
        }
    }
}
