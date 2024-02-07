﻿using CCL.Creator.Inspector;
using CCL.Creator.Utility;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Indicators;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    public class LampWizard : EditorWindow
    {
        private static LampWizard? _instance;

        [MenuItem("GameObject/CCL/Add Lamp", false, 10)]
        public static void ShowWindow(MenuCommand command)
        {
            _instance = GetWindow<LampWizard>();
            _instance.Refresh((GameObject)command.context);
            _instance.titleContent = new GUIContent("CCL - Lamp Wizard");
            _instance.Show();
        }

        [MenuItem("GameObject/CCL/Add Lamp", true, 10)]
        public static bool OnContextMenuValidate()
        {
            return Selection.activeGameObject;
        }

        [Serializable]
        private class Settings : ScriptableObject
        {
            public enum ReaderType
            {
                Port,
                Fuse,
                BrakeIssues,
            }

            public GameObject TargetObject = null!;
            public string IndicatorName = "newLamp";

            public ReaderType TrackedValueType = ReaderType.Port;

            public DVPortValueType PortFilter;
            [PortId]
            public string SourcePortId;

            [FuseId]
            public string SourceFuseId;

            [FuseId]
            public string FuseId;
        }

        private Settings _settings;

        private void Refresh(GameObject target)
        {
            _settings = CreateInstance<Settings>();
            _settings.TargetObject = target;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");
            EditorStyles.label.wordWrap = true;

            var serialized = new SerializedObject(_settings);

            EditorGUILayout.PropertyField(serialized.FindProperty(nameof(Settings.IndicatorName)));

            var valueProp = serialized.FindProperty(nameof(Settings.TrackedValueType));
            EditorGUILayout.PropertyField(valueProp);
            var valueType = (Settings.ReaderType)Enum.GetValues(typeof(Settings.ReaderType)).GetValue(valueProp.enumValueIndex);

            if (valueType == Settings.ReaderType.Fuse)
            {
                EditorGUILayout.Space();
                var idRect = EditorGUILayout.GetControlRect();
                FuseIdEditor.RenderProperty(
                    idRect,
                    serialized.FindProperty(nameof(Settings.SourceFuseId)),
                    new GUIContent("Tracked Fuse"),
                    _settings.TargetObject.transform);
            }
            else if (valueType == Settings.ReaderType.Port)
            {
                EditorGUILayout.Space();

                var filterProp = serialized.FindProperty(nameof(Settings.PortFilter));
                EditorGUILayout.PropertyField(filterProp);

                var filterType = (DVPortValueType)Enum.GetValues(typeof(DVPortValueType)).GetValue(filterProp.enumValueIndex);

                var idRect = EditorGUILayout.GetControlRect();
                var filter = new PortIdAttribute(filterType);

                PortIdEditor.RenderProperty(
                    idRect,
                    serialized.FindProperty(nameof(Settings.SourcePortId)),
                    new GUIContent("Tracked Port"),
                    _settings.TargetObject.transform,
                    filter);
            }

            var fuseRect = EditorGUILayout.GetControlRect();
            FuseIdEditor.RenderProperty(
                fuseRect,
                serialized.FindProperty(nameof(Settings.FuseId)),
                new GUIContent("Power Fuse"),
                _settings.TargetObject.transform);


            serialized.ApplyModifiedProperties();
            EditorGUILayout.Space(18);

            if (GUILayout.Button("Add Lamp"))
            {
                CreateLamp(_settings);
                Close();
                return;
            }

            EditorGUILayout.EndVertical();
        }

        private static void CreateLamp(Settings settings)
        {
            var holder = new GameObject(settings.IndicatorName);
            holder.transform.SetParent(settings.TargetObject.transform, false);

            var controller = holder.AddComponent<LampControlProxy>();

            switch (settings.TrackedValueType)
            {
                case Settings.ReaderType.Fuse:
                    var fuseReader = holder.AddComponent<LampFuseReaderProxy>();
                    fuseReader.fuseId = settings.SourceFuseId;
                    fuseReader.powerFuseId = settings.FuseId;
                    break;

                case Settings.ReaderType.Port:
                    var portReader = holder.AddComponent<LampPortReaderProxy>();
                    portReader.portId = settings.SourcePortId;
                    portReader.fuseId = settings.FuseId;
                    break;

                case Settings.ReaderType.BrakeIssues:
                    var brakeReader = holder.AddComponent<LampBrakeIssueReaderProxy>();
                    brakeReader.lampPowerFuseId = settings.FuseId;
                    break;
            }

            var emitterHolder = new GameObject("emitter");
            emitterHolder.transform.SetParent(holder.transform, false);
            var indicator = emitterHolder.AddComponent<IndicatorEmissionProxy>();

            controller.lampInd = indicator;

            // TODO: Glare component/renderer

            EditorUtility.SetDirty(settings.TargetObject);
            Selection.activeObject = holder;
            EditorHelpers.SaveAndRefresh();
        }
    }
}
