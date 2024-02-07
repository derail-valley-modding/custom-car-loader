using CCL.Types.Components.HUD;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CCL.Creator.Editor.HUD
{
    [CustomEditor(typeof(VanillaHudLayout))]
    internal class VanillaHudLayoutEditor : UnityEditor.Editor
    {
        private VanillaHudLayout _layout = null!;

        private static bool _showBasic;
        private static bool _showBrake;
        private static bool _showSteam;
        private static bool _showCab;
        private static bool _showMechanical;

        public override void OnInspectorGUI()
        {
            _layout = (VanillaHudLayout)target;

            DrawBasicSlots(_layout.Settings);
            DrawBrakeSlots(_layout.Settings);
            DrawSteamSlots(_layout.Settings);
            DrawCabSlots(_layout.Settings);
            DrawMechanicalSlots(_layout.Settings);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Auto setup", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Auto setup will only set most mandatory controls. " +
                "Wipers, lights, etc will remain as they are. The layout can be edited at " +
                "any time, you don't have to stick with the defaults.", MessageType.Info);
            EditorGUILayout.Space();

            if (GUILayout.Button(new GUIContent("Set to DE", "Sets up the HUD for a diesel-electric locomotive")))
            {
                VanillaHudSettings.SetToDE(_layout.Settings);
            }
            if (GUILayout.Button(new GUIContent("Set to DH", "Sets up the HUD for a diesel-hydraulic locomotive")))
            {
                VanillaHudSettings.SetToDH(_layout.Settings);
            }
            if (GUILayout.Button(new GUIContent("Set to DM", "Sets up the HUD for a diesel-mechanical locomotive")))
            {
                VanillaHudSettings.SetToDM(_layout.Settings);
            }
            if (GUILayout.Button(new GUIContent("Set to S", "Sets up the HUD for a steam locomotive")))
            {
                VanillaHudSettings.SetToS(_layout.Settings);
            }
            if (GUILayout.Button(new GUIContent("Reset", "Resets the HUD to no controls")))
            {
                _layout.Settings = new VanillaHudSettings();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_layout);
                EditorSceneManager.MarkSceneDirty(_layout.gameObject.scene);
            }
        }

        private static void DrawBasicSlots(VanillaHudSettings settings)
        {
            _showBasic = EditorGUILayout.Foldout(_showBasic, "Basic controls");

            if (!_showBasic)
            {
                return;
            }

            for (int i = 0; i < VanillaHudSettings.BasicControlCount; i++)
            {
                var s = settings.BasicControls[i];

                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.LabelField($"Slot {i}");
                    EditorGUI.indentLevel++;

                    switch (i)
                    {
                        case 0:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Amps" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Throttle", "Regulator" });
                            break;
                        case 1:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "TM Temp", "Oil Temp" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Reverser", "Cutoff" });
                            break;
                        case 2:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Gearbox A" });
                            break;
                        case 3:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Speedometer" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Gearbox B" });
                            break;
                        case 4:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "RPM" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Turbine RPM", "Voltage" });
                            s.Value3 = EditorGUILayout.Popup(s.Value3, new string[] { "None", "Power" });
                            break;
                        case 5:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Wheelslip" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Sand" });
                            break;
                        default:
                            break;
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }            
        }

        private static void DrawBrakeSlots(VanillaHudSettings settings)
        {
            _showBrake = EditorGUILayout.Foldout(_showBrake, "Brake controls");

            if (!_showBrake)
            {
                return;
            }

            for (int i = 0; i < VanillaHudSettings.BrakeControlCount; i++)
            {
                var s = settings.BrakeControls[i];

                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.LabelField($"Slot {i}");
                    EditorGUI.indentLevel++;

                    switch (i)
                    {
                        case 0:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Brake Pipe" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Self-lapping Brake", "Non Self-lapping Brake" });
                            break;
                        case 1:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Main Reservoir" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Independent Brake" });
                            break;
                        case 2:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Brake Cylinder" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Dynamic Brake" });
                            break;
                        case 3:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Release Cylinder" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Handbrake" });
                            s.Value3 = EditorGUILayout.Popup(s.Value3, new string[] { "None", "Handbrake Hold" });
                            break;
                        default:
                            break;
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }
        }

        private static void DrawSteamSlots(VanillaHudSettings settings)
        {
            _showSteam = EditorGUILayout.Foldout(_showSteam, "Steam controls");

            if (!_showSteam)
            {
                return;
            }

            for (int i = 0; i < VanillaHudSettings.SteamControlCount; i++)
            {
                var s = settings.SteamControls[i];

                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.LabelField($"Slot {i}");
                    EditorGUI.indentLevel++;

                    switch (i)
                    {
                        case 0:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Steam Meter" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Cylinder Cocks" });
                            break;
                        case 1:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Loco Water Meter" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Injector" });
                            break;
                        case 2:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Loco Coal Meter" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Damper" });
                            break;
                        case 3:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Fire Temperature" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Blower" });
                            break;
                        case 4:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Shovel" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Firedoor" });
                            break;
                        case 5:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Light Firebox" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Water Dump" });
                            break;
                        case 6:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Coal Dump" });
                            break;
                        case 7:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Dynamo" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Air Pump" });
                            s.Value3 = EditorGUILayout.Popup(s.Value3, new string[] { "None", "Lubricator" });
                            break;
                        default:
                            break;
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }
        }

        private static void DrawCabSlots(VanillaHudSettings settings)
        {
            _showCab = EditorGUILayout.Foldout(_showCab, "Cab controls");

            if (!_showCab)
            {
                return;
            }

            for (int i = 0; i < VanillaHudSettings.CabControlCount; i++)
            {
                var s = settings.CabControls[i];

                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.LabelField($"Slot {i}");
                    EditorGUI.indentLevel++;

                    switch (i)
                    {
                        case 0:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Fuel Level" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Wipers", "DM3 Wipers" });
                            break;
                        case 1:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Oil Level" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Cab Light", "Cab + Dash Light", "Cab Light + Gear Light" });
                            break;
                        case 2:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Sand Level" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Headlights", "DM3 Headlights" });
                            break;
                        case 3:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Bell Button", "Tender Water" });
                            s.Value2 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Headlights", "DM3 Headlights", "Steam Bell" });
                            break;
                        case 4:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Horn", "Whistle + Tender Coal" });
                            break;
                        default:
                            break;
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }
        }

        private static void DrawMechanicalSlots(VanillaHudSettings settings)
        {
            _showMechanical = EditorGUILayout.Foldout(_showMechanical, "Mechanical controls");

            if (!_showMechanical)
            {
                return;
            }

            for (int i = 0; i < VanillaHudSettings.MechanicalControlCount; i++)
            {
                var s = settings.MechanicalControls[i];

                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.LabelField($"Slot {i}");
                    EditorGUI.indentLevel++;

                    switch (i)
                    {
                        case 0:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Pantograph" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Cab Orientation" });
                            break;
                        case 1:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "TM Offline" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Starter Fuse" });
                            s.Value3 = EditorGUILayout.Popup(s.Value3, new string[] { "None", "Electrics Fuse" });
                            s.Value4 = EditorGUILayout.Popup(s.Value4, new string[] { "None", "Traction Motor Fuse" });
                            break;
                        case 2:
                            s.Value1 = EditorGUILayout.Popup(s.Value1, new string[] { "None", "Alerter" });
                            s.Value2 = EditorGUILayout.Popup(s.Value2, new string[] { "None", "Starter" });
                            s.Value3 = EditorGUILayout.Popup(s.Value3, new string[] { "None", "Fuel Cutoff" });
                            break;
                        default:
                            break;
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }
        }
    }
}
