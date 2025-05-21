using CCL.Creator.Utility;
using CCL.Types.Proxies;
using CCL.Types.Proxies.VFX;
using UnityEditor;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(MonoBehaviourWithVehicleDefaults), true), CanEditMultipleObjects]
    internal class MonoBehaviourWithVehicleDefaultsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorHelpers.DrawLocoDefaultsButtons(target);
        }
    }

    [CustomEditor(typeof(SteamSmokeParticlePortReaderProxy), true), CanEditMultipleObjects]
    internal class SteamSmokeParticlePortReaderProxyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorHelpers.DrawLocoDefaultsButtons(target);
        }
    }
}
