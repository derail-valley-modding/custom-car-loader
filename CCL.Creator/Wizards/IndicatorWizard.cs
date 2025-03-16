using CCL.Creator.Inspector;
using CCL.Creator.Utility;
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

            public DVPortValueType PortFilter;
            [PortId]
            public string PortId;

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

            EditorGUILayout.Space();

            var fuseRect = EditorGUILayout.GetControlRect();
            FuseIdEditor.RenderProperty(
                fuseRect,
                serialized.FindProperty(nameof(Settings.FuseId)),
                new GUIContent("Power Fuse"),
                _settings.TargetObject.transform);


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

        private static IndicatorProxy AddIndicatorScript(IndicatorType type, GameObject location)
        {
            switch (type)
            {
                case IndicatorType.Gauge:
                    return location.AddComponent<IndicatorGaugeProxy>();
                case IndicatorType.LaggingGauge:
                    return location.AddComponent<IndicatorGaugeLaggingProxy>();
                case IndicatorType.Emission:
                    return location.AddComponent<IndicatorEmissionProxy>();
                case IndicatorType.Scaler:
                    return location.AddComponent<IndicatorScalerProxy>();
                case IndicatorType.Slider:
                    return location.AddComponent<IndicatorSliderProxy>();
                case IndicatorType.ModelChanger:
                    return location.AddComponent<IndicatorModelChangerProxy>();
                default:
                    throw new NotImplementedException();
            }
        }

        private static void CreateIndicator(Settings settings)
        {
            var holder = new GameObject(settings.IndicatorName);
            holder.transform.SetParent(settings.TargetObject.transform, false);

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
    }

    internal enum IndicatorValueType
    {
        PortValue,
        BrakePipe,
        BrakeReservoir,
        BrakeCylinder,
    }
}
