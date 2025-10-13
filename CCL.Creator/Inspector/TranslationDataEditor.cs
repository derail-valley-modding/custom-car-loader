using DVLangHelper.Data;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomPropertyDrawer(typeof(TranslationData))]
    public class TranslationDataEditor : GridPropertyDrawer<TranslationData>
    {
        protected SerializedProperty? Items => Property?.FindPropertyRelative(nameof(TranslationData.Items));

        protected override int Columns => 2;

        protected override int Rows => (Items?.arraySize ?? 0) + 1;

        protected override void DrawCells(Rect bounds)
        {
            var itemsProp = Items;
            if (itemsProp == null) return;

            int i;
            for (i = 0; i < itemsProp.arraySize; i++)
            {
                var item = itemsProp.GetArrayElementAtIndex(i);

                var langSpinnerRect = GetCellPosition(bounds, 0, i);
                var langProp = item.FindPropertyRelative(nameof(TranslationItem.Language));

                if (i == 0)
                {
                    EditorGUI.LabelField(langSpinnerRect, "English");
                }
                else
                {
                    EditorGUI.PropertyField(langSpinnerRect, langProp, GUIContent.none);
                }

                var valueFieldRect = GetCellPosition(bounds, 1, i);
                var valueProp = item.FindPropertyRelative(nameof(TranslationItem.Value));
                EditorGUI.PropertyField(valueFieldRect, valueProp, GUIContent.none);
            }

            var cell = GetCellPosition(bounds, 0, i);
            if (GUI.Button(cell, "+"))
            {
                itemsProp.arraySize += 1;
            }

            cell = GetCellPosition(bounds, 1, i);
            if (GUI.Button(cell, "-"))
            {
                if (itemsProp.arraySize > 1)
                {
                    itemsProp.arraySize -= 1;
                }
            }
        }
    }
}
