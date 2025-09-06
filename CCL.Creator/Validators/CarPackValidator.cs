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

        private class TableWidths
        {
            public const float Padding = 4.0f;

            public float Name = 30.0f;
            public float Result = 20.0f;
            public float Button = 20.0f;
            public float Message = 100.0f;

            public void ResizeName(float value)
            {
                Name = Mathf.Max(Name, value);
            }

            public void ResizeName(string text)
            {
                ResizeName(TextSize(text));
            }

            public void ResizeResult(float value)
            {
                Result = Mathf.Max(Result, value);
            }

            public void ResizeResult(string text)
            {
                ResizeResult(TextSize(text));
            }

            public void ResizeButton(float value)
            {
                Button = Mathf.Max(Button, value);
            }

            public void ResizeButton(string text)
            {
                ResizeButton(TextSize(text));
            }

            public void ResizeMessage(float value)
            {
                Message = Mathf.Max(Message, value);
            }

            public void ResizeMessage(string text)
            {
                ResizeMessage(TextSize(text));
            }

            public GUILayoutOption[] ToOptions()
            {
                return new[] { GUILayout.Width(Name), GUILayout.Width(Result), GUILayout.Width(Button), GUILayout.Width(Message) };
            }

            public static float TextSize(string text) => EditorStyles.label.CalcSize(new GUIContent(text)).x + Padding;
        }

        private static CarPackValidator s_window = null!;

        private CustomCarPack? _pack;
        private ValidationResult _packResult = null!;
        private List<CarResults> _results = new List<CarResults>();
        private bool _needsCorrections = false;
        private bool _failed = false;
        private Vector2 _scroll = Vector2.zero;
        private TableWidths _widths = null!;

        public static void ValidateExport(CustomCarPack pack)
        {
            s_window = GetWindow<CarPackValidator>();
            s_window.titleContent = new GUIContent("Pack Validator");

            if (!pack)
            {
                Debug.LogError("Car pack is null, cannot validate");
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

            ValidatePack(pack);

            foreach (var car in pack.Cars)
            {
                ValidateCar(car);
            }

            _widths = new TableWidths();

            _widths.ResizeButton(" Go ");

            foreach (var name in Enum.GetNames(typeof(ResultStatus)))
            {
                _widths.ResizeResult(name);
            }

            foreach (var entry in _packResult.Entries)
            {
                _widths.ResizeName(GetName(entry));
                _widths.ResizeMessage(GetMessage(entry));
            }

            foreach (var carResult in _results)
            {
                foreach (var result in carResult.Results)
                {
                    foreach (var entry in result.Entries)
                    {
                        _widths.ResizeName(GetName(entry));
                        _widths.ResizeMessage(GetMessage(entry));
                    }
                }
            }
        }

        private void ValidatePack(CustomCarPack pack)
        {
            _packResult = new ValidationResult("Pack Fields");

            if (string.IsNullOrWhiteSpace(pack.PackId))
            {
                _packResult.Fail("Pack ID is empty", pack);
            }

            if (string.IsNullOrWhiteSpace(pack.PackName))
            {
                _packResult.Fail("Pack name is empty", pack);
            }

            if (string.IsNullOrWhiteSpace(pack.Author))
            {
                _packResult.Fail("Author is empty", pack);
            }

            if (pack.Cars.Length == 0)
            {
                _packResult.Fail("Pack has no cars", pack);
            }

            if (pack.Cars.Any(x => x is null))
            {
                _packResult.Fail("There are missing cars in the pack", pack);
            }

            if (pack.ExtraModels.Any(x => x is null))
            {
                _packResult.Fail("There are missing models in the pack", pack);
            }

            if (pack.PaintSubstitutions.Any(x => x is null))
            {
                _packResult.Fail("There are missing paints in the pack", pack);
            }

            _needsCorrections |= _packResult.IsCorrectable;
            _failed |= _packResult.AnyFailure;
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
            if (_pack == null || _packResult == null) return;

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            EditorGUILayout.BeginVertical();

            GUILayout.BeginVertical("box");
            EditorHelpers.DrawHeader("Pack");

            DrawResults(_packResult.Entries, _widths);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            foreach (var list in _results)
            {
                DrawSingleCarResults(list, _widths);
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

        private static void DrawSingleCarResults(CarResults results, TableWidths widths)
        {
            GUILayout.BeginVertical("box");
            EditorHelpers.DrawHeader(results.CarId);

            DrawResults(results.Results.SelectMany(x => x.Entries), widths);
            
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private static void DrawResults(IEnumerable<ResultEntry> entries, TableWidths widths)
        {
            var options = widths.ToOptions();

            foreach (var result in entries)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(GetName(result), options[0]);
                
                GUILayout.Label(GetStatus(result), EditorHelpers.StyleWithTextColour(result.StatusColor), options[1]);

                DrawContextButton(result, options[2]);

                GUILayout.Label(GetMessage(result), options[3]);

                EditorGUILayout.EndHorizontal();
            }
        }

        private static void DrawContextButton(ResultEntry result, GUILayoutOption width)
        {
            if (result.HasContext)
            {
                // Opens the offending object/component in the inspector.
                if (GUILayout.Button("Go", width))
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
                if (GUILayout.Button("Go", width))
                {
                    SettingsService.OpenProjectSettings(result.SettingsContext);
                }
            }
            else
            {
                // Padding so the buttons aren't misaligned.
                GUILayout.Label(" ", width, GUILayout.Height(EditorGUIUtility.singleLineHeight + 1));
            }
        }

        private static string GetName(ResultEntry entry) => $"{entry.TestName}: ";
        private static string GetStatus(ResultEntry entry) => Enum.GetName(typeof(ResultStatus), entry.Status);
        private static string GetMessage(ResultEntry entry) => entry.Message;
    }
}
