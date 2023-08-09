using CCL.Creator.Validators;
using CCL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator
{
    public class TrainCarValidator : EditorWindow
    {
        private static TrainCarValidator? window = null;

        private readonly List<Result> results = new List<Result>();
        private CustomCarType? carType = null;
        private bool validationPassed = false;

        Vector2 scrollPos = Vector2.zero;

        delegate IEnumerable<Result> CarValidatorFunc(CustomCarType carType);

        private class Validator
        {
            public readonly string TestName;
            public readonly CarValidatorFunc Func;

            public Validator(string testName, CarValidatorFunc func)
            {
                TestName = testName;
                Func = func;
            }
        }

        private static readonly List<Validator> _validators;

        static TrainCarValidator()
        {
            _validators = new List<Validator>();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    if (method.GetCustomAttribute<CarValidatorAttribute>() is CarValidatorAttribute attr)
                    {
                        var testMethod = (CarValidatorFunc)method.CreateDelegate(typeof(CarValidatorFunc));
                        _validators.Add(new Validator(attr.TestName, testMethod));
                    }
                }
            }
        }

        private void OnGUI()
        {
            Color defaultText = EditorStyles.label.normal.textColor;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.BeginHorizontal();

            // Test Names
            GUILayout.BeginVertical();
            foreach (Result result in results)
            {
                GUILayout.Label($"{result.Name}: ");
            }
            GUILayout.EndVertical();

            // Statuses
            GUILayout.BeginVertical();
            foreach (Result result in results)
            {
                GUI.skin.label.normal.textColor = result.StatusColor;
                GUILayout.Label(Enum.GetName(typeof(ResultStatus), result.Status));
            }
            GUILayout.EndVertical();

            // Messages
            GUI.skin.label.normal.textColor = defaultText;
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            foreach (Result result in results)
            {
                GUILayout.Label(" " + result.Message);
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();

            if (validationPassed)
            {
                GUILayout.Label("Validation passed :)");

                if (GUILayout.Button("Continue Export", GUILayout.Height(GUI.skin.button.lineHeight * 2)))
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
                GUILayout.Label("CCL Wiki: https://github.com/katycat5e/DVCustomCarLoader/wiki");

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
            carType = carSetup;

            foreach (var result in _validators.SelectMany(RunTest))
            {
                results.Add(result);
                if (result.Status == ResultStatus.Failed)
                {
                    validationPassed = false;
                }
            }
        }

        private IEnumerable<Result> RunTest(Validator validator)
        {
            foreach (var result in validator.Func(carType!))
            {
                result.Name = validator.TestName;
                yield return result;
            }
        }
    }
}
