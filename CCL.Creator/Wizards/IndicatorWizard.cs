using CCL.Creator.Inspector;
using CCL.Creator.Utility;
using CCL.Types;
using CCL.Types.Components.Indicators;
using CCL.Types.Proxies.Indicators;
using CCL.Types.Proxies.Ports;
using System;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    public class IndicatorWizard : EditorWindow
    {
        private static IndicatorWizard? _instance;

        [MenuItem("GameObject/CCL/Add Indicator", false, MenuOrdering.Cab.Indicator)]
        public static void ShowWindow(MenuCommand command)
        {
            _instance = GetWindow<IndicatorWizard>();
            _instance.Refresh((GameObject)command.context);
            _instance.titleContent = new GUIContent("CCL - Indicator Wizard");
            _instance.Show();
        }

        [MenuItem("GameObject/CCL/Add Indicator", true, MenuOrdering.Cab.Indicator)]
        public static bool OnContextMenuValidate()
        {
            return Selection.activeGameObject;
        }

        [Serializable]
        private class Settings : ScriptableObject
        {
            public GameObject TargetObject = null!;
            public string IndicatorName = "newIndicator";
            public IndicatorType IndicatorType;
            public IndicatorValueType ValueType;
            public bool AutomaticReparenting = true;

            public DVPortValueType PortFilter;
            [PortId]
            public string PortId = string.Empty;
            [FuseId]
            public string FuseId = string.Empty;
        }

        private Settings _settings = null!;

        private void Refresh(GameObject target)
        {
            _settings = CreateInstance<Settings>();
            _settings.TargetObject = target;
        }

        private void OnGUI()
        {
            using (new WordWrapScope(true))
            {
                EditorGUILayout.BeginVertical("box");

                var serialized = new SerializedObject(_settings);

                EditorGUILayout.PropertyField(serialized.FindProperty(nameof(Settings.IndicatorName)));
                EditorGUILayout.PropertyField(serialized.FindProperty(nameof(Settings.IndicatorType)));

                var valueProp = serialized.FindProperty(nameof(Settings.ValueType));
                EditorGUILayout.PropertyField(valueProp);

                var valueType = (IndicatorValueType)Enum.GetValues(typeof(IndicatorValueType)).GetValue(valueProp.enumValueIndex);
                if (valueType == IndicatorValueType.PortValue)
                {
                    EditorGUILayout.Space();

                    var filterProp = serialized.FindProperty(nameof(Settings.PortFilter));
                    EditorGUILayout.PropertyField(filterProp);

                    var filterType = (DVPortValueType)Enum.GetValues(typeof(DVPortValueType)).GetValue(filterProp.enumValueIndex);

                    var idRect = EditorGUILayout.GetControlRect();
                    var filter = new PortIdAttribute(filterType);

                    PortIdEditor.RenderProperty(
                        idRect,
                        serialized.FindProperty(nameof(Settings.PortId)),
                        new GUIContent("Source Port"),
                        _settings.TargetObject.transform,
                        filter);
                }

                EditorGUILayout.PropertyField(serialized.FindProperty(nameof(Settings.AutomaticReparenting)));
                EditorGUILayout.Space();

                var fuseRect = EditorGUILayout.GetControlRect();
                FuseIdEditor.RenderProperty(
                    fuseRect,
                    serialized.FindProperty(nameof(Settings.FuseId)),
                    new GUIContent("Power Fuse"),
                    _settings.TargetObject.transform,
                    new FuseIdAttribute());


                serialized.ApplyModifiedProperties();
                EditorGUILayout.Space(18);

                if (GUILayout.Button("Add Indicator"))
                {
                    CreateIndicator(_settings);
                    Close();
                    return;
                }

                EditorGUILayout.EndVertical();
            }
        }

        private static IndicatorProxy AddIndicatorScript(IndicatorType type, GameObject location) => type switch
        {
            IndicatorType.Gauge => location.AddComponent<IndicatorGaugeProxy>(),
            IndicatorType.LaggingGauge => location.AddComponent<IndicatorGaugeLaggingProxy>(),
            IndicatorType.Emission => location.AddComponent<IndicatorEmissionProxy>(),
            IndicatorType.Scaler => location.AddComponent<IndicatorScalerProxy>(),
            IndicatorType.Slider => location.AddComponent<IndicatorSliderProxy>(),
            IndicatorType.ModelChanger => location.AddComponent<IndicatorModelChangerProxy>(),
            IndicatorType.Shader => location.AddComponent<IndicatorShaderValueProxy>(),

            IndicatorType.DeltaGauge => location.AddComponent<IndicatorGaugeDelta>(),
            IndicatorType.CustomShader => location.AddComponent<IndicatorShaderCustomValue>(),
            IndicatorType.TMP => location.AddComponent<IndicatorTMP>(),

            _ => throw new NotImplementedException(),
        };

        private static void CreateIndicator(Settings settings)
        {
            var parent = settings.TargetObject.transform.parent;
            var holder = new GameObject(settings.IndicatorName);

            AddIndicatorScript(settings.IndicatorType, holder);

            switch (settings.ValueType)
            {
                case IndicatorValueType.PortValue:
                    var portReader = holder.AddComponent<IndicatorPortReaderProxy>();
                    portReader.portId = settings.PortId;
                    portReader.fuseId = settings.FuseId;
                    break;

                case IndicatorValueType.BrakePipe:
                    var pipeReader = holder.AddComponent<IndicatorBrakePipeReaderProxy>();
                    pipeReader.fuseId = settings.FuseId;
                    break;

                case IndicatorValueType.BrakeReservoir:
                    var resReader = holder.AddComponent<IndicatorBrakeReservoirReaderProxy>();
                    resReader.fuseId = settings.FuseId;
                    break;

                case IndicatorValueType.BrakeCylinder:
                    var cylReader = holder.AddComponent<IndicatorBrakeCylinderReaderProxy>();
                    cylReader.fuseId = settings.FuseId;
                    break;
            }

            if (parent != null && settings.AutomaticReparenting)
            {
                holder.transform.SetParent(parent, false);
                Utilities.CopyTransform(settings.TargetObject.transform, holder.transform);
                settings.TargetObject.transform.SetParent(holder.transform);
                settings.TargetObject.transform.ResetLocal();
            }
            else
            {
                holder.transform.SetParent(settings.TargetObject.transform, false);
            }

            EditorUtility.SetDirty(settings.TargetObject);
            Selection.activeObject = holder;
            EditorHelpers.SaveAndRefresh();
        }
    }

    internal enum IndicatorType
    {
        Gauge,
        LaggingGauge,
        Emission,
        Scaler,
        Slider,
        ModelChanger,
        Shader,
        // Custom.
        DeltaGauge = 1000,
        CustomShader,
        TMP
    }

    internal enum IndicatorValueType
    {
        PortValue,
        BrakePipe,
        BrakeReservoir,
        BrakeCylinder,
    }
}
