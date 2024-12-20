using CCL.Creator.Wizards;
using CCL.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Validators
{
    public class TrainCarValidator : EditorWindow
    {
        private static readonly AggregateCatalog _catalog;
        private static TrainCarValidator? window = null;

        static TrainCarValidator()
        {
            _catalog = new AggregateCatalog();
            _catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
        }

        [ImportMany]
        private IEnumerable<ICarValidator>? _steps = new ICarValidator[0];
        private readonly List<ICarValidator> SortedSteps;

        private readonly List<ValidationResult> results = new List<ValidationResult>();
        private CustomCarType? carType = null;
        private bool validationPassed = false;
        private bool correctionsNeeded = false;

        Vector2 scrollPos = Vector2.zero;

        public TrainCarValidator()
        {
            var container = new CompositionContainer(_catalog);
            container.ComposeParts(this);

            SortedSteps = new List<ICarValidator>(_steps);
            SortedSteps.Sort();
        }

        private void OnGUI()
        {
            Color defaultText = EditorStyles.label.normal.textColor;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.BeginHorizontal();

            var entries = results.SelectMany(r => r.Entries).ToList();

            // Test Names
            GUILayout.BeginVertical();
            foreach (var result in entries)
            {
                // All rows need 1 extra pixel to align with the buttons.
                GUILayout.Label($"{result.TestName}: ", GUILayout.Height(EditorGUIUtility.singleLineHeight + 1));
            }
            GUILayout.EndVertical();

            // Statuses
            GUILayout.BeginVertical();
            foreach (var result in entries)
            {
                GUI.skin.label.normal.textColor = result.StatusColor;
                GUILayout.Label(Enum.GetName(typeof(ResultStatus), result.Status), GUILayout.Height(EditorGUIUtility.singleLineHeight + 1));
            }
            GUILayout.EndVertical();

            // Quick link
            GUI.skin.label.normal.textColor = defaultText;
            GUILayout.BeginVertical();
            GUILayout.Space(2);
            foreach (var result in entries)
            {
                if (result.HasContext)
                {
                    // Opens the offending object/component in the inspector.
                    if (GUILayout.Button("Go"))
                    {
                        // Open the prefab if needed.
                        var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(result.Context);

                        if (!string.IsNullOrEmpty(prefabPath))
                        {
                            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath));
                        }

                        Selection.activeObject = result.Context;
                    }
                }
                else if (result.HasSettingContext)
                {
                    // Opens project settings.
                    if (GUILayout.Button("Go"))
                    {
                        SettingsService.OpenProjectSettings(result.SettingsContext);
                    }
                }
                else
                {
                    // Padding so the buttons aren't misaligned.
                    GUILayout.Label(" ", GUILayout.Height(EditorGUIUtility.singleLineHeight + 1));
                }
            }
            GUILayout.EndVertical();

            // Messages
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            foreach (var result in entries)
            {
                GUILayout.Label(" " + result.Message, GUILayout.Height(EditorGUIUtility.singleLineHeight + 1));
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();

            if (correctionsNeeded)
            {
                if (GUILayout.Button("Open CCL Documentation"))
                {
                    Application.OpenURL("https://github.com/derail-valley-modding/custom-car-loader/wiki");
                }
            }

            GUILayout.Space(GUI.skin.button.lineHeight);
            if (validationPassed)
            {
                GUILayout.Label("Validation passed :)");

                if (GUILayout.Button("Continue Export"))
                {
                    ExportTrainCar.ShowWindow(carType!);
                    Close();
                }
                if (GUILayout.Button("Cancel"))
                {
                    Close();
                }
            }
            else
            {
                GUILayout.Label("Validation failed - correct errors and try again");

                if (GUILayout.Button("OK"))
                {
                    Close();
                }
            }
        }


        public static void ValidateExport(CustomCarType carSetup)
        {
            window = GetWindow<TrainCarValidator>();
            window.titleContent = new GUIContent("Car Validator");

            if (!carSetup)
            {
                Debug.LogError("Train car setup is null, cannot validate");
                return;
            }

            window.DoValidation(carSetup);

            window.Show();
        }

        private void DoValidation(CustomCarType carSetup)
        {
            results.Clear();
            validationPassed = true;
            correctionsNeeded = false;
            carType = carSetup;
            bool criticalFail = false;

            foreach (var validator in SortedSteps)
            {
                var result = validator.Validate(carSetup);
                results.Add(result);

                if (result.IsCorrectable)
                {
                    correctionsNeeded = true;
                }

                if (result.AnyFailure)
                {
                    validationPassed = false;
                }
                if (result.Status == ResultStatus.Critical)
                {
                    criticalFail = true;
                }

                if (criticalFail) return;
            }
        }
    }
}
