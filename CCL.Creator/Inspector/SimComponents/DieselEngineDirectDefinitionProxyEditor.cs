using CCL.Creator.Utility;
using CCL.Types.Proxies.Simulation.Diesel;
using UnityEditor;

namespace CCL.Creator.Inspector.SimComponents
{
    [CustomEditor(typeof(DieselEngineDirectDefinitionProxy)), CanEditMultipleObjects]
    internal class DieselEngineDirectDefinitionProxyEditor : Editor
    {
        private DieselEngineDirectDefinitionProxy _proxy = null!;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _proxy = (DieselEngineDirectDefinitionProxy)target;

            EditorHelpers.DrawHeader("Calculated Values");
            EditorGUILayout.LabelField("Minimum RPM", $"{_proxy.engineRpmIdle * 0.4f:F0}");

            EditorHelpers.DrawLocoDefaultsButtons(target);
        }
    }
}
