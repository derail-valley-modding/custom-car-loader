using CCL.Creator.Utility;
using CCL.Types.Components;
using CCL.Types.Proxies.Headlights;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    internal class HeadlightWizard : EditorWindow
    {
        private class Settings
        {
            public string[] WhiteNames = new string[0];
            public string[] RedNames = new string[0];

            public static Settings Copy(Settings other)
            {
                string[] white = new string[other.WhiteNames.Length];
                string[] red = new string[other.RedNames.Length];

                Array.Copy(other.WhiteNames, white, white.Length);
                Array.Copy(other.RedNames, red, red.Length);

                return new Settings { WhiteNames = white, RedNames = red };
            }
        }

        private enum LightType
        {
            High,
            Low,
            Red
        }

        private static HeadlightWizard? s_instance;

        public GameObject TargetObject = null!;

        private Settings _settingsR = null!;
        private Settings _settingsF = null!;

        [MenuItem("GameObject/CCL/Add Headlights", false, MenuOrdering.Body.Headlights)]
        public static void ShowWindow(MenuCommand command)
        {
            s_instance = GetWindow<HeadlightWizard>();

            s_instance._settingsF = new Settings();
            s_instance._settingsR = new Settings();
            s_instance.TargetObject = (GameObject)command.context;
            s_instance.titleContent = new GUIContent("CCL - Headlight Wizard");
            s_instance.Show();
        }

        [MenuItem("GameObject/CCL/Add Headlights", true, MenuOrdering.Body.Headlights)]
        public static bool OnContextMenuValidate()
        {
            return Selection.activeGameObject;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");
            EditorStyles.label.wordWrap = true;

            EditorHelpers.DrawHeader("Front Lights");
            EditorHelpers.DrawStringArray("White Light Names", ref _settingsF.WhiteNames, true);
            EditorHelpers.DrawStringArray("Red Light Names", ref _settingsF.RedNames, true);

            if (GUILayout.Button("Copy Front Lights To Rear"))
            {
                _settingsR = Settings.Copy(_settingsF);
            }

            EditorHelpers.DrawHeader("Rear Lights");
            EditorHelpers.DrawStringArray("White Light Names", ref _settingsR.WhiteNames, true);
            EditorHelpers.DrawStringArray("Red Light Names", ref _settingsR.RedNames, true);

            EditorGUILayout.Space(18);

            if (GUILayout.Button("Create"))
            {
                CreateTemplate();
                return;
            }

            EditorGUILayout.EndVertical();
        }

        private void CreateTemplate()
        {
            var root = new GameObject("[headlights]");
            root.transform.parent = TargetObject.transform;
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;

            int whiteF = _settingsF.WhiteNames.Length;
            int redF = _settingsF.RedNames.Length;
            int whiteR = _settingsR.WhiteNames.Length;
            int redR = _settingsR.RedNames.Length;

            var main = root.AddComponent<HeadlightsMainControllerProxy>();
            var opti = root.AddComponent<CarLightsOptimizerProxy>();

            if (whiteF > 0 || whiteR > 0)
            {
                var beam = root.AddComponent<HeadlightBeamControllerProxy>();
                beam.headlightsMainController = main;
                opti.beamController = beam;
            }

            var front = new GameObject("FrontSide").transform;
            front.parent = root.transform;
            front.localPosition = Vector3.zero;
            front.localRotation = Quaternion.identity;

            main.headlightSetupsFront = new HeadlightSetup[6];

            for (int i = 0; i < 6; i++)
            {
                main.headlightSetupsFront[i] = front.gameObject.AddComponent<HeadlightSetup>();
            }

            var rear = new GameObject("RearSide").transform;
            rear.parent = root.transform;
            rear.localPosition = Vector3.zero;
            rear.localRotation = Quaternion.identity;

            main.headlightSetupsRear = new HeadlightSetup[6];

            for (int i = 0; i < 6; i++)
            {
                main.headlightSetupsRear[i] = rear.gameObject.AddComponent<HeadlightSetup>();
            }

            main.headlightSetupsFront[2].mainOffSetup = true;
            main.headlightSetupsRear[2].mainOffSetup = true;

            if (whiteF > 0)
            {
                var subControllerParent = GetSubControllerTransform(front.transform);

                List<HeadlightProxy> highs = new List<HeadlightProxy>();
                List<HeadlightProxy> lows = new List<HeadlightProxy>();

                foreach (var item in _settingsF.WhiteNames)
                {
                    highs.Add(CreateHeadlight(front.transform, LightType.High, item));
                    lows.Add(CreateHeadlight(front.transform, LightType.Low, item));
                }

                var highController = AddSubController(subControllerParent, "WhiteHigh", true);
                highController.headlights = highs.ToArray();
                var lowController = AddSubController(subControllerParent, "WhiteLow", true);
                lowController.headlights = lows.ToArray();

                var light = new GameObject("LightHigh").AddComponent<Light>();
                light.transform.parent = front.transform;
                light.transform.localPosition = Vector3.zero;
                light.transform.localEulerAngles = new Vector3(1, 0, 0);
                highController.lightSources = new[] { light };
                CopyVanillaLight.ApplyProperties(light, VanillaLight.LocoHeadlightsHigh);

                light = new GameObject("LightLow").AddComponent<Light>();
                light.transform.parent = front.transform;
                light.transform.localPosition = Vector3.zero;
                light.transform.localEulerAngles = new Vector3(8, 0, 0);
                lowController.lightSources = new[] { light };
                CopyVanillaLight.ApplyProperties(light, VanillaLight.LocoHeadlightsLow);

                main.headlightSetupsFront[5].subControllers = new[] { highController };
                main.headlightSetupsFront[5].setting = HeadlightSetup.HeadlightSetting.HeadlightSetting03;
                main.headlightSetupsFront[4].subControllers = new[] { lowController };
                main.headlightSetupsFront[4].setting = HeadlightSetup.HeadlightSetting.HeadlightSetting02;
                main.headlightSetupsFront[3].subControllers = new[] { lowController };
                main.headlightSetupsFront[3].setting = HeadlightSetup.HeadlightSetting.HeadlightSetting01;
            }

            if (redF > 0)
            {
                var subControllerParent = GetSubControllerTransform(front.transform);

                List<HeadlightProxy> reds = new List<HeadlightProxy>();

                foreach (var item in _settingsF.RedNames)
                {
                    reds.Add(CreateHeadlight(front.transform, LightType.Red, item));
                }

                var controller = AddSubController(subControllerParent, "Red", true);
                controller.headlights = reds.ToArray();

                main.headlightSetupsFront[0].subControllers = new[] { controller };
                main.headlightSetupsFront[0].setting = HeadlightSetup.HeadlightSetting.HeadlightSetting02;
                main.headlightSetupsFront[1].subControllers = new[] { controller };
                main.headlightSetupsFront[1].setting = HeadlightSetup.HeadlightSetting.HeadlightSetting01;
            }

            if (whiteR > 0)
            {
                var subControllerParent = GetSubControllerTransform(rear.transform);

                List<HeadlightProxy> highs = new List<HeadlightProxy>();
                List<HeadlightProxy> lows = new List<HeadlightProxy>();

                foreach (var item in _settingsR.WhiteNames)
                {
                    highs.Add(CreateHeadlight(rear.transform, LightType.High, item));
                    lows.Add(CreateHeadlight(rear.transform, LightType.Low, item));
                }

                var highController = AddSubController(subControllerParent, "WhiteHigh", false);
                highController.headlights = highs.ToArray();
                var lowController = AddSubController(subControllerParent, "WhiteLow", false);
                lowController.headlights = lows.ToArray();

                var light = new GameObject("LightHigh").AddComponent<Light>();
                light.transform.parent = rear.transform;
                light.transform.localPosition = Vector3.zero;
                light.transform.localEulerAngles = new Vector3(1, 0, 0);
                highController.lightSources = new[] { light };
                CopyVanillaLight.ApplyProperties(light, VanillaLight.LocoHeadlightsHigh);

                light = new GameObject("LightLow").AddComponent<Light>();
                light.transform.parent = rear.transform;
                light.transform.localPosition = Vector3.zero;
                light.transform.localEulerAngles = new Vector3(8, 0, 0);
                lowController.lightSources = new[] { light };
                CopyVanillaLight.ApplyProperties(light, VanillaLight.LocoHeadlightsLow);

                main.headlightSetupsRear[5].subControllers = new[] { highController };
                main.headlightSetupsRear[5].setting = HeadlightSetup.HeadlightSetting.HeadlightSetting03;
                main.headlightSetupsRear[4].subControllers = new[] { lowController };
                main.headlightSetupsRear[4].setting = HeadlightSetup.HeadlightSetting.HeadlightSetting02;
                main.headlightSetupsRear[3].subControllers = new[] { lowController };
                main.headlightSetupsRear[3].setting = HeadlightSetup.HeadlightSetting.HeadlightSetting01;
            }

            if (redR > 0)
            {
                var subControllerParent = GetSubControllerTransform(rear.transform);

                List<HeadlightProxy> reds = new List<HeadlightProxy>();

                foreach (var item in _settingsR.RedNames)
                {
                    reds.Add(CreateHeadlight(rear.transform, LightType.Red, item));
                }

                var controller = AddSubController(subControllerParent, "Red", false);
                controller.headlights = reds.ToArray();

                main.headlightSetupsRear[0].subControllers = new[] { controller };
                main.headlightSetupsRear[0].setting = HeadlightSetup.HeadlightSetting.HeadlightSetting02;
                main.headlightSetupsRear[1].subControllers = new[] { controller };
                main.headlightSetupsRear[1].setting = HeadlightSetup.HeadlightSetting.HeadlightSetting01;
            }

            AssetHelper.SaveAsset(root);
            return;
        }

        private static Transform GetSubControllerTransform(Transform parent)
        {
            var t = parent.Find("SubControllers");

            if (t == null)
            {
                t = new GameObject("SubControllers").transform;
                t.parent = parent;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
            }

            return t;
        }

        private static HeadlightProxy CreateHeadlight(Transform parent, LightType type, string name)
        {
            var light = new GameObject($"Headlights{name}{GetName(type)}").AddComponent<HeadlightProxy>();
            light.transform.parent = parent;
            light.transform.localPosition = Vector3.zero;
            light.transform.localRotation = Quaternion.identity;

            return light;

            static string GetName(LightType type) => type switch
            {
                LightType.High => "High",
                LightType.Low => "Low",
                LightType.Red => "Red",
                _ => string.Empty,
            };
        }

        private static HeadlightsSubControllerStandardProxy AddSubController(Transform parent, string name, bool front)
        {
            var controller = new GameObject(name).AddComponent<HeadlightsSubControllerStandardProxy>();
            controller.transform.parent = parent;
            controller.transform.localPosition = Vector3.zero;
            controller.transform.localRotation = Quaternion.identity;

            if (front)
            {
                controller.isFront = true;
                controller.multipleUnityDependent = HeadlightsSubControllerBaseProxy.HeadlightMUDependency.Front;
            }
            else
            {
                controller.isFront = false;
                controller.multipleUnityDependent = HeadlightsSubControllerBaseProxy.HeadlightMUDependency.Rear;
            }

            return controller;
        }
    }
}
