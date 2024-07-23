using CCL.Types.Components;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(CopyVanillaLight))]
    internal class CopyVanillaLightEditor : Editor
    {
        private CopyVanillaLight _comp = null!;

        public override void OnInspectorGUI()
        {
            _comp = (CopyVanillaLight)target;

            base.OnInspectorGUI();

            EditorGUILayout.HelpBox("This component will apply the properties to the light in the same GameObject.\n" +
                "It will also attempt to add a LightShadowQualityProxy if one is used by the original light.", MessageType.Info);

            if (GUILayout.Button("Apply"))
            {
                _comp.ApplyProperties();
            }
        }
    }
}
