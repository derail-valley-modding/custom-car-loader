using CCL.Creator.Utility;
using CCL.Types.Proxies.Simulation.Steam;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(BoilerDefinitionProxy)), CanEditMultipleObjects]
    internal class BoilerDefinitionProxyEditor : Editor
    {
        private BoilerDefinitionProxy _proxy = null!;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _proxy = (BoilerDefinitionProxy)target;

            var radius = _proxy.diameter * 0.5f;
            var volume = radius * radius * Mathf.PI * _proxy.length * _proxy.capacityMultiplier;

            EditorHelpers.DrawHeader("Calculated Values");
            EditorGUILayout.LabelField("Capacity", $"{volume * 1000.0f:F2} L");
            EditorGUILayout.LabelField("Spawn Water % At 1 bar", $"{_proxy.spawnWaterLevel * 0.001f / volume * 100.0f:F2}%");

            EditorHelpers.DrawLocoDefaultsButtons(target);
        }
    }
}
