using CCL.Creator.Utility;
using CCL.Types;
using CCL.Types.Proxies.Simulation.Electric;
using UnityEditor;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(TractionMotorSetDefinitionProxy)), CanEditMultipleObjects]
    internal class TractionMotorSetDefinitionProxyEditor : Editor
    {
        private TractionMotorSetDefinitionProxy _proxy = null!;

        private float NumberTMs => _proxy.numberOfTractionMotors;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _proxy = (TractionMotorSetDefinitionProxy)target;

            var powerLoss = (_proxy.maxAmpsPerTm * _proxy.maxAmpsPerTm * _proxy.externalResistance) +
                (_proxy.maxAmpsPerTm * _proxy.maxAmpsPerTm * _proxy.motorResistance);
            var dbPower = _proxy.dynamicBrakeMaxCurrent * _proxy.dynamicBrakeMaxCurrent * _proxy.dynamicBrakeGridResistance;

            // Convert to kW for display.
            powerLoss *= Units.ToKilo;
            dbPower *= Units.ToKilo;

            EditorHelpers.DrawHeader("Calculated Values (per TM)");
            EditorGUILayout.LabelField("Power Loss (Half/Max Current)", $"{powerLoss * 0.25f:F2}/{powerLoss:F2} kW");
            EditorGUILayout.LabelField("Dynamic Brake Power", $"{dbPower:F2} kW");

            powerLoss *= NumberTMs;
            dbPower *= NumberTMs;
            EditorHelpers.DrawHeader("Calculated Values (Total)");
            EditorGUILayout.LabelField("Power Loss (Half/Max Current)", $"{powerLoss * 0.25f:F2}/{powerLoss:F2} kW");
            EditorGUILayout.LabelField("Dynamic Brake Power", $"{dbPower:F2} kW");

            EditorHelpers.DrawLocoDefaultsButtons(target);
        }
    }
}
