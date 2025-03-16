using CCL.Creator.Inspector;
using CCL.Creator.Utility;
using CCL.Types.Proxies.Indicators;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    public class LabelWizard : EditorWindow
    {
        private static LabelWizard? _instance;

        [MenuItem("GameObject/CCL/Add Label", false, MenuOrdering.Cab.Label)]
        public static void ShowWindow(MenuCommand command)
        {
            _instance = GetWindow<LabelWizard>();
            _instance.TargetObject = (GameObject)command.context;
            _instance.titleContent = new GUIContent("CCL - Label Wizard");
            _instance.Show();
        }

        [MenuItem("GameObject/CCL/Add Label", true, MenuOrdering.Cab.Label)]
        public static bool OnContextMenuValidate()
        {
            return Selection.activeGameObject;
        }

        public GameObject TargetObject = null!;
        private string _name = "newLabel";
        private int _selectedModelIdx = 0;

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");
            EditorStyles.label.wordWrap = true;

            _name = EditorGUILayout.TextField("Label Name:", _name);
            _selectedModelIdx = EditorGUILayout.Popup(new GUIContent("Model Type:"), _selectedModelIdx, LabelLocalizerEditor.ModelOptionNames);

            EditorGUILayout.Space(18);

            if (GUILayout.Button("Add Label"))
            {
                var modelType = LabelLocalizerEditor.ModelOptions[_selectedModelIdx];
                CreateLabel(TargetObject, _name, modelType);
                Close();
                return;
            }

            EditorGUILayout.EndVertical();
        }

        private static void CreateLabel(GameObject target, string name, LabelModelType modelType)
        {
            var holder = new GameObject(name);
            holder.transform.SetParent(target.transform, false);

            var localizer = holder.AddComponent<LabelLocalizer>();
            localizer.ModelType = modelType;

            if ((modelType == LabelModelType.None) || modelType.HasFlag(LabelModelType.CustomText))
            {
                var textMesh = holder.AddComponent<TextMeshPro>();
                textMesh.SetText(name);
                textMesh.fontSizeMin = 0.1f;
                textMesh.enableAutoSizing = true;
                textMesh.horizontalAlignment = HorizontalAlignmentOptions.Center;
                textMesh.verticalAlignment = VerticalAlignmentOptions.Middle;
                
                var rect = holder.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(0.065f, 0.015f);
            }

            EditorUtility.SetDirty(target);
            Selection.activeGameObject = holder;
            EditorHelpers.SaveAndRefresh();
        }
    }
}
