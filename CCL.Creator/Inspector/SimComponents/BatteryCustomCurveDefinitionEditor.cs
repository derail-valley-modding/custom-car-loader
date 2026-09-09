using CCL.Creator.Utility;
using CCL.Types.Components.Simulation.Electric;
using UnityEditor;

namespace CCL.Creator.Inspector.SimComponents
{
    [CustomEditor(typeof(BatteryCustomCurveDefinition))]
    internal class BatteryCustomCurveDefinitionEditor : Editor
    {
        private BatteryCustomCurveDefinition _proxy = null!;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _proxy = (BatteryCustomCurveDefinition)target;

            float minVoltage = 0;
            float maxVoltage = 0;

            if (_proxy.chargeToVoltageCurve != null)
            {
                minVoltage = _proxy.numSeriesCells * _proxy.chargeToVoltageCurve[0].value;
                maxVoltage = _proxy.numSeriesCells * _proxy.chargeToVoltageCurve[_proxy.chargeToVoltageCurve.length - 1].value;
            }

            EditorHelpers.DrawHeader("Calculated Values");
            EditorGUILayout.LabelField("Min Voltage", $"{minVoltage:F2} V");
            EditorGUILayout.LabelField("Max Voltage", $"{maxVoltage:F2} V");

            EditorHelpers.DrawLocoDefaultsButtons(target);
        }
    }
}
