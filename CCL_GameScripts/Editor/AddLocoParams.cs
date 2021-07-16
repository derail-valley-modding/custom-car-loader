using System.Collections;
using System.Collections.Generic;
using CCL_GameScripts;
using UnityEditor;
using UnityEngine;

public class AddLocoParams : EditorWindow
{
    #region Static

    private static AddLocoParams window = null;

    [InitializeOnLoadMethod]
    static void Init()
    {
        TrainCarSetup.LaunchLocoSetupWindow = ShowWindow;
    }

    public static void ShowWindow( TrainCarSetup trainCarSetup )
    {
        window = GetWindow<AddLocoParams>();
        window.trainScript = trainCarSetup;
    }

    #endregion

    #region Instance

    private TrainCarSetup trainScript;
    private LocoSimTemplate LocoType;

    private void ResetWindow()
    {
        LocoType = LocoSimTemplate.Shunter;
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        LocoType = (LocoSimTemplate)EditorGUILayout.EnumPopup("Template Loco type:", LocoType);

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();

        if( GUILayout.Button("Cancel") )
        {
            Close();
            return;
        }

        if( GUILayout.Button("Apply Template") )
        {
            ApplyTemplate();
            Close();
            return;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void ApplyTemplate()
    {
        var obj = trainScript.gameObject;

        if( (LocoType == LocoSimTemplate.DE6) || (LocoType == LocoSimTemplate.Shunter) )
        {
            SimParamsDiesel simParams = obj.GetComponent<SimParamsDiesel>();
            if( !simParams )
            {
                simParams = obj.AddComponent<SimParamsDiesel>();
            }

            DamageConfigDiesel dmgConfig = obj.GetComponent<DamageConfigDiesel>();
            if( !dmgConfig )
            {
                dmgConfig = obj.AddComponent<DamageConfigDiesel>();
            }

            if( LocoType == LocoSimTemplate.DE6 )
            {
                simParams.ApplyDE6Defaults();
                dmgConfig.ApplyDE6Defaults();
            }
            else
            {
                simParams.ApplyShunterDefaults();
                dmgConfig.ApplyShunterDefaults();
            }
        }
    }

    #endregion
}

public enum LocoSimTemplate
{
    Shunter, DE6, SH282
}
