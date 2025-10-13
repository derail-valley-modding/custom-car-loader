using CCL.Creator.Utility;
using CCL.Types.Proxies.Simulation.Steam;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(ReciprocatingSteamEngineDefinitionProxy)), CanEditMultipleObjects]
    internal class ReciprocatingSteamEngineDefinitionProxyEditor : Editor
    {
        private ReciprocatingSteamEngineDefinitionProxy _proxy = null!;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _proxy = (ReciprocatingSteamEngineDefinitionProxy)target;

            var radius = _proxy.cylinderBore * 0.5f;
            var volume = radius * radius * Mathf.PI * _proxy.pistonStroke;

            EditorHelpers.DrawHeader("Calculated Values");
            EditorGUILayout.LabelField("Cylinder Volume", $"{volume * 1000f:F0} L");

            EditorHelpers.DrawLocoDefaultsButtons(target);
        }
    }
}
