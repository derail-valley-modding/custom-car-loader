using CCL.Types;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Editor
{
    [CustomPropertyDrawer(typeof(LoadableCargoEntry))]
    internal class LoadableCargoEntryDrawer : PropertyDrawer
    {
        private static GUIContent amountC = new GUIContent("Amount");
        private static GUIContent customC = new GUIContent("Use Custom Cargo");
        private static GUIContent cargoTC = new GUIContent("Cargo Type");
        private static GUIContent modelsC = new GUIContent("Model Variants");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var amount = property.FindPropertyRelative(nameof(LoadableCargoEntry.AmountPerCar));
            var custom = property.FindPropertyRelative(nameof(LoadableCargoEntry.UseCustomCargo));
            var cargoT = property.FindPropertyRelative(nameof(LoadableCargoEntry.CargoType));
            var models = property.FindPropertyRelative(nameof(LoadableCargoEntry.ModelVariants));

            EditorGUI.BeginProperty(position, label, property);

            Rect controlPos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Vector2 offset = new Vector2(0, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(controlPos, label);
            controlPos.position += offset;

            using (new EditorGUI.IndentLevelScope())
            {
                amount.floatValue = EditorGUI.FloatField(controlPos, amountC, amount.floatValue);
                controlPos.position += offset;
                custom.boolValue = EditorGUI.Toggle(controlPos, customC, custom.boolValue);
                controlPos.position += offset;

                if (custom.boolValue)
                {
                    cargoT.intValue = EditorGUI.IntField(controlPos, cargoTC, cargoT.intValue);
                }
                else
                {
                    cargoT.intValue = (int)(BaseCargoType)EditorGUI.EnumPopup(controlPos, cargoTC, (BaseCargoType)cargoT.intValue);
                }

                controlPos.position += offset;
                EditorGUI.PropertyField(controlPos, models, modelsC);
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
