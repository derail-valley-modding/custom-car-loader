using CCL.Creator.Utility;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(VanillaLightCopy))]
    internal class CopyVanillaLightEditor : Editor
    {
        private VanillaLightCopy _comp = null!;

        public override void OnInspectorGUI()
        {
            _comp = (VanillaLightCopy)target;

            base.OnInspectorGUI();

            EditorGUILayout.HelpBox("This component will apply the properties to the light in the same GameObject.\n" +
                "It will also attempt to add a LightShadowQualityProxy if one is used by the original light.\n" +
                "You should remove this component before exporting.", MessageType.Info);

            if (GUILayout.Button("Apply"))
            {
                _comp.ApplyProperties();
            }
        }
    }
}
