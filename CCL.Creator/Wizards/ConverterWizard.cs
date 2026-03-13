using CCL.Types;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    internal class ConverterWizard : EditorWindow
    {
        [MenuItem("CCL/Converter", priority = MenuOrdering.MenuBar.Converter)]
        public static void ShowWindow()
        {
            var window = GetWindow<ConverterWizard>();
            window.Show();
            window.titleContent = new GUIContent("CCL - Converter");

            foreach (var item in window._conversions)
            {
                item.UpdateAll();
            }
        }

        private ConvertBase[] _conversions = new[]
        {
            new ConvertBase("Length", "Metres",
                new ExtraUnit("Inches", Units.InchToMetre),
                new ExtraUnit("Feet", Units.FootToMetre),
                new ExtraUnit("Miles", Units.MileToMetre),
                new ExtraUnit("Kilometres", Units.FromKilo))
            { Show = true },

            new ConvertBase("Speed", "Kilometres/hour",
                new ExtraUnit("Miles/hour", Units.MPHtoKMH),
                new ExtraUnit("Metres/second", Units.KMHtoMS)),

            new ConvertBase("Force", "Newtons",
                new ExtraUnit("Pounds-Force", Units.LBFtoNewton),
                new ExtraUnit("Kilograms-Force", Units.KGFtoNewton)),

            new ConvertBase("Pressure", "Pascals",
                new ExtraUnit("Bars", Units.BarToPascal),
                new ExtraUnit("PSI", Units.PSItoBar),
                new ExtraUnit("kgf/cm²", Units.KGFCM2toPascal),
                new ExtraUnit("Atmospheres", Units.KGFCM2toPascal)),
        };

        private void OnGUI()
        {
            foreach (var conversion in _conversions)
            {
                DrawConversion(conversion);
            }
        }

        private static void DrawConversion(ConvertBase conversion)
        {
            using (new GUIColorScope(newBackground: GUI.backgroundColor * 1.1f))
            {
                conversion.Show = EditorGUILayout.BeginFoldoutHeaderGroup(conversion.Show, conversion.Title);
            }

            if (!conversion.Show)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return;
            }

            EditorGUI.indentLevel++;

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                conversion.Value = EditorGUILayout.FloatField(conversion.Unit, conversion.Value);

                if (scope.changed)
                {
                    conversion.UpdateAll();
                }
            }

            foreach (var item in conversion.Units)
            {
                using var scope = new EditorGUI.ChangeCheckScope();
                item.Value = EditorGUILayout.FloatField(item.Unit, item.Value);

                if (scope.changed)
                {
                    conversion.Value = item.Value * item.ConversionFactor;
                    conversion.UpdateAll();
                }
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private class ConvertBase
        {
            public bool Show;
            public string Title;
            public string Unit;
            public float Value = 1;
            public ExtraUnit[] Units;

            public ConvertBase(string title, string unit, params ExtraUnit[] units)
            {
                Title = title;
                Unit = unit;
                Units = units;
            }

            public void UpdateAll()
            {
                foreach (var item in Units)
                {
                    item.Value = Value / item.ConversionFactor;
                }
            }
        }

        private class ExtraUnit
        {
            public string Unit;
            public float Value;
            public float ConversionFactor;

            public ExtraUnit(string unit, float conversionFactor)
            {
                Unit = unit;
                ConversionFactor = conversionFactor;
            }
        }
    }
}
