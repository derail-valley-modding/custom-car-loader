using CCL.Creator.Utility;
using CCL.Types.Proxies.Simulation.Electric;
using UnityEditor;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(TractionMotorSetDefinitionProxy)), CanEditMultipleObjects]
    internal class TractionMotorSetDefinitionProxyEditor : Editor
    {
        private TractionMotorSetDefinitionProxy _proxy = null!;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _proxy = (TractionMotorSetDefinitionProxy)target;

            var power = _proxy.dynamicBrakeMaxCurrent * _proxy.dynamicBrakeMaxCurrent * _proxy.dynamicBrakeGridResistance;

            EditorHelpers.DrawHeader("Calculated Values");
            EditorGUILayout.LabelField("Dynamic Brake Power/TM", $"{power * 0.001f:F2} kW");
            EditorGUILayout.LabelField("Dynamic Brake Max Power", $"{_proxy.numberOfTractionMotors * power * 0.001f:F2} kW");

            EditorHelpers.DrawLocoDefaultsButtons(target);
        }
    }
}
