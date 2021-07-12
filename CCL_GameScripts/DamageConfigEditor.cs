using UnityEditor;
using UnityEngine;

namespace CCL_GameScripts
{
    [CustomEditor(typeof(DamageConfigDiesel))]
    public class DamageConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var script = (DamageConfigDiesel)target;
            if( GUILayout.Button("Apply DE6 Defaults") )
            {
                script.ApplyDE6Defaults();
            }
            if( GUILayout.Button("Apply Shunter Defaults") )
            {
                script.ApplyShunterDefaults();
            }
        }
    }
}