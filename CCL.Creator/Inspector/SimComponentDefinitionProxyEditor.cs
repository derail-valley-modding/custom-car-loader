using CCL.Creator.Utility;
using CCL.Types.Proxies.Ports;
using UnityEditor;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(SimComponentDefinitionProxy), true)]
    internal class SimComponentDefinitionProxyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorHelpers.DrawLocoDefaultsButtons(target);
        }
    }
}
