using CCL.Creator.Utility;
using CCL.Creator.Wizards;
using CCL.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Validators
{
    internal class CarPackValidator : EditorWindow
    {
        private static readonly AggregateCatalog s_catalog;

        [ImportMany]
        private IEnumerable<ICarValidator>? _steps = new ICarValidator[0];
        private readonly List<ICarValidator> SortedSteps;

        static CarPackValidator()
        {
            s_catalog = new AggregateCatalog();
            s_catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
        }

        private CarPackValidator()
        {
            var container = new CompositionContainer(s_catalog);
            container.ComposeParts(this);

            SortedSteps = new List<ICarValidator>(_steps);
            SortedSteps.Sort();
        }

        private class CarResults
        {
            public string CarId;
            public List<ValidationResult> Results;

            public CarResults(string carId, List<ValidationResult> results)
            {
                CarId = carId;
                Results = results;
            }
        }

        private static CarPackValidator s_window = null!;

        private CustomCarPack? _pack;
        private List<CarResults> _results = new List<CarResults>();
        private bool _needsCorrections = false;
        private bool _failed = false;
        private Vector2 _scroll = Vector2.zero;

        public static void ValidateExport(CustomCarPack pack)
        {
            s_window = GetWindow<CarPackValidator>();
            s_window.titleContent = new GUIContent("Pack Validator");

            if (!pack)
            {
                Debug.LogError("Train car setup is null, cannot validate");
                return;
            }

            s_window.DoValidation(pack);
            s_window.Show();
        }

        private void DoValidation(CustomCarPack pack)
        {
            _results.Clear();

            _pack = pack;
            _needsCorrections = false;
            _failed = false;

            foreach (var car in pack.Cars)
            {
                ValidateCar(car);
            }
        }

        private void ValidateCar(CustomCarType car)
        {
            var results = new List<ValidationResult>();

            foreach (var validator in SortedSteps)
            {
                var result = validator.Validate(car);
                results.Add(result);

                _needsCorrections |= result.IsCorrectable;
                _failed |= result.AnyFailure;

                if (result.Status == ResultStatus.Critical)
                {
                    Debug.LogError($"Critical failure while exporting car {car.id}");
                    break;
                }
            }

            _results.Add(new CarResults(car.id, results));
        }

        private void OnGUI()
        {
            if (_pack == null) return;

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            EditorGUILayout.BeginVertical();

            foreach (var list in _results)
            {
                DrawSingleCarResults(list);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            if (_needsCorrections)
            {
                EditorGUILayout.Space();

                if (GUILayout.Button("Open CCL Documentation"))
                {
                    Application.OpenURL("https://github.com/derail-valley-modding/custom-car-loader/wiki");
                }
            }

            EditorGUILayout.Space();

            if (_failed)
            {
                GUILayout.Label("Validation failed - correct errors and try again");

                if (GUILayout.Button("OK"))
                {
                    Close();
                }
            }
            else
            {
                GUILayout.Label("Validation passed :)");

                if (GUILayout.Button("Continue Export"))
                {
                    ExportPackWizard.ShowWindow(_pack);
                    Close();
                }
                if (GUILayout.Button("Cancel"))
                {
                    Close();
                }
            }
        }

        private static void DrawSingleCarResults(CarResults results)
        {
            EditorHelpers.DrawHeader(results.CarId);

            GUILayout.BeginHorizontal();

            var entries = results.Results.SelectMany(x => x.Entries);

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
                using (new GUIColorScope(result.StatusColor))
                {
                    GUILayout.Label(Enum.GetName(typeof(ResultStatus), result.Status), GUILayout.Height(EditorGUIUtility.singleLineHeight + 1));
                }
            }
            GUILayout.EndVertical();

            // Quick link
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
            EditorGUILayout.Space();
        }
    }
}
