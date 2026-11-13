using CCL.Creator.Utility;
using CCL.Types.Components.Simulation.Electric;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector.SimComponents
{
    [CustomEditor(typeof(BatteryCustomCurveDefinition))]
    internal class BatteryCustomCurveDefinitionEditor : Editor
    {
        private BatteryCustomCurveDefinition _proxy = null!;
        private float _nominalVoltage;
        private bool _hasCalculatedNominalVoltage;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _proxy = (BatteryCustomCurveDefinition)target;

            float minVoltage = 0;
            float maxVoltage = 0;
            float totalResistance = 0;

            if (_proxy.chargeToVoltageCurve != null)
            {
                minVoltage = _proxy.numSeriesCells * _proxy.chargeToVoltageCurve[0].value;
                maxVoltage = _proxy.numSeriesCells * _proxy.chargeToVoltageCurve[_proxy.chargeToVoltageCurve.length - 1].value;
                totalResistance = _proxy.numSeriesCells * _proxy.internalResistance;
            }

            EditorHelpers.DrawHeader("Calculated Values");
            EditorGUILayout.LabelField("Min Voltage", $"{minVoltage:F2} V");
            EditorGUILayout.LabelField("Max Voltage", $"{maxVoltage:F2} V");
            EditorGUILayout.LabelField("Total Resistance", $"{totalResistance:F2} Ω");

            if (_hasCalculatedNominalVoltage)
            {
                EditorGUILayout.LabelField("Nominal Cell Voltage", $"{_nominalVoltage:F2} V");
                EditorGUILayout.LabelField("Nominal Pack Voltage", $"{_nominalVoltage * _proxy.numSeriesCells:F2} V");
            }

            if (GUILayout.Button("Calculate Nominal Voltages"))
            {
                if (_proxy.chargeToVoltageCurve != null)
                {
                    _nominalVoltage = CalculateNominalCellVoltage(_proxy.chargeToVoltageCurve);
                    _hasCalculatedNominalVoltage = true;
                }
            }

            EditorHelpers.DrawLocoDefaultsButtons(target);
        }

        public float CalculateNominalCellVoltage(AnimationCurve curve, int samples = 100)
        {
            float sum = 0f;

            for (int i = 0; i < samples; i++)
            {
                float soc = i / (samples - 1f);
                sum += curve.Evaluate(soc);
            }

            return sum / samples;
        }
    }
}
