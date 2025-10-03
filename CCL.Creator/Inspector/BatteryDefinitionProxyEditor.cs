using CCL.Creator.Utility;
using CCL.Types.Proxies.Simulation.Electric;
using UnityEditor;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(BatteryDefinitionProxy))]
    internal class BatteryDefinitionProxyEditor : Editor
    {
        private BatteryDefinitionProxy _proxy = null!;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _proxy = (BatteryDefinitionProxy)target;

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.EnumPopup("Chemistry", _proxy.chemistry);
            }

            float minVoltage;
            float maxVoltage;

            switch (_proxy.chemistry)
            {
                case BatteryChemistry.LeadAcid:
                    minVoltage = _proxy.numSeriesCells * 1.94f;
                    maxVoltage = _proxy.numSeriesCells * 2.15f;
                    break;
                default:
                    minVoltage = 0;
                    maxVoltage = 0;
                    break;
            }

            EditorHelpers.DrawHeader("Calculated Values");
            EditorGUILayout.LabelField("Min Voltage", $"{minVoltage:F2} V");
            EditorGUILayout.LabelField("Max Voltage", $"{maxVoltage:F2} V");

            EditorHelpers.DrawLocoDefaultsButtons(target);
        }
    }
}
