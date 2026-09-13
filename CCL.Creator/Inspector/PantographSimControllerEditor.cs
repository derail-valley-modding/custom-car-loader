using UnityEditor;

using CCL.Types.Components.Controllers;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(PantographSimController))]
    internal class PantographSimControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component requires '1500 V DC Catenary' mod or its derivative to collect power.\n" +
                "If the catenary mod is not installed or active, the entire map will be regarded as unelectrified.", MessageType.Info);
            base.OnInspectorGUI();
        }
    }
}
