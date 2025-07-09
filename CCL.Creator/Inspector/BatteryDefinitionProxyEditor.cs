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
        }
    }
}
