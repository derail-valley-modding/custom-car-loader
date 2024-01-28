using CCL.Types;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Editor
{
    [CustomPropertyDrawer(typeof(LoadableCargoEntry))]
    internal class LoadableCargoEntryDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var amount = property.FindPropertyRelative(nameof(LoadableCargoEntry.AmountPerCar));
            var cargoT = property.FindPropertyRelative(nameof(LoadableCargoEntry.CargoType));
            var custom = property.FindPropertyRelative(nameof(LoadableCargoEntry.CustomCargoId));
            var models = property.FindPropertyRelative(nameof(LoadableCargoEntry.ModelVariants));

            EditorGUI.BeginProperty(position, label, property);

            Rect controlPos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Vector2 offset = new Vector2(0, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(controlPos, label);
            controlPos.position += offset;

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUI.PropertyField(controlPos, amount);
                controlPos.position += offset;
                EditorGUI.PropertyField(controlPos, cargoT);
                controlPos.position += offset;

                GUI.enabled = cargoT.intValue == (int)BaseCargoType.Custom;
                EditorGUI.PropertyField(controlPos, custom);
                controlPos.position += offset;
                GUI.enabled = true;

                EditorGUI.PropertyField(controlPos, models);
                controlPos.position += offset;

                if (models.isExpanded)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        int length = EditorGUI.DelayedIntField(controlPos, "Size", models.arraySize);
                        models.arraySize = length;
                        controlPos.position += offset;

                        for (int i = 0; i < length; i++)
                        {
                            EditorGUI.PropertyField(controlPos, models.GetArrayElementAtIndex(i), new GUIContent($"Element {i}"));
                            controlPos.position += offset;
                        }
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var models = property.FindPropertyRelative(nameof(LoadableCargoEntry.ModelVariants));
            return EditorGUIUtility.singleLineHeight * 4 + EditorGUI.GetPropertyHeight(models, true);
        }
    }
}
