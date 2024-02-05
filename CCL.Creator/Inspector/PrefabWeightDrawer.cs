using CCL.Types.Components;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomPropertyDrawer(typeof(PrefabWeight))]
    internal class PrefabWeightDrawer : PropertyDrawer
    {
        private const float WeightWidth = 65;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var prefab = property.FindPropertyRelative(nameof(PrefabWeight.Prefab));
            var weight = property.FindPropertyRelative(nameof(PrefabWeight.Weight));

            EditorGUI.BeginProperty(position, label, property);

            var width = position.width;
            Rect controlPos = new Rect(position.x, position.y, width - WeightWidth, EditorGUIUtility.singleLineHeight);
            Vector2 offset = new Vector2(width - WeightWidth, 0);

            EditorGUI.PropertyField(controlPos, prefab);
            controlPos.position += offset;
            controlPos.width = WeightWidth;
            EditorGUI.PropertyField(controlPos, weight, GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
