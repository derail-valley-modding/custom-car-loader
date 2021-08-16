using UnityEditor;
using UnityEngine;

namespace CCL_GameScripts
{
    [CustomEditor(typeof(SimParamsDiesel))]
    public class SimParamsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var script = (SimParamsDiesel)target;
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