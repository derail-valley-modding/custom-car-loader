using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Diesel
{
    public class HydraulicTransmissionDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized
    {
        [Header("Torque Transmission")]
        public float outputTorqueLimit;

        public HydraulicConfigDefinition[] configs;
        [SerializeField, HideInInspector]
        private string? _configJson;

        [Header("Hydrodynamic Brake")]
        public float hydroDynamicBrakeTorqueCapacity;

        [Header("Transitions")]
        public AnimationCurve pumpRpmFillCurve = AnimationCurve.EaseInOut(200f, 0f, 400f, 1f);
        public float couplingFillTime = 1f;

        [Header("Damage")]
        public float overheatingThreshold = 120f;
        public float overheatingMaxTime = 12f;
        public float overheatingDamagePerDegreePerSecond = 0.1f;
        public float overheatingExplosionDamage = 300f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.HEAT_RATE, "HEAT_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TORQUE, "INPUT_SHAFT_TORQUE"),
            new PortDefinition(DVPortType.OUT, DVPortValueType.TORQUE, "OUTPUT_SHAFT_TORQUE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "HYDRO_DYNAMIC_BRAKE_EFFECT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "GEAR_RATIO"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TORQUE, "PUMP_TORQUE_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "TRANSMISSION_ENGAGED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "TURBINE_RPM"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "TURBINE_RPM_NORMALIZED"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "MECHANICAL_PT_HEALTH_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.DAMAGE, "GENERATED_DAMAGE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "IS_BROKEN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "ACTIVE_CONFIGURATION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "SPEED_RATIO"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "INPUT_SHAFT_POWER"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "OUTPUT_SHAFT_POWER"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "EFFICIENCY"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "HYDRODYNAMIC_BRAKE"),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "REVERSER"),
            new PortReferenceDefinition(DVPortValueType.RPM, "INPUT_SHAFT_RPM"),
            new PortReferenceDefinition(DVPortValueType.RPM, "MAX_RPM"),
            new PortReferenceDefinition(DVPortValueType.RPM, "OUTPUT_SHAFT_RPM"),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TEMPERATURE"),
        };

        public void OnValidate()
        {
            _configJson = JSONObject.ToJson(configs);
        }

        public void AfterImport()
        {
            configs = JSONObject.FromJson(_configJson, () => Array.Empty<HydraulicConfigDefinition>());
        }

        #region Defaults

        [MethodButton(nameof(ApplyDM3Defaults), "Apply DM3 Defaults")]
        [MethodButton(nameof(ApplyDH4Defaults), "Apply DH4 Defaults")]
        [RenderMethodButtons]
        public bool renderButtons;

        public void ApplyDM3Defaults()
        {
            outputTorqueLimit = 25000;
            hydroDynamicBrakeTorqueCapacity = 0;
            couplingFillTime = 1;

            overheatingThreshold = 120;
            overheatingMaxTime = 6;
            overheatingDamagePerDegreePerSecond = 0.1f;
            overheatingExplosionDamage = 1000;

            configs = new[]
            {
                new HydraulicConfigDefinition()
                {
                    torqueCapacity = 15,
                    stallTorqueMultiplier = 1,
                    couplingSpeedRatio = 1,
                    maxEfficiency = 1,
                    hasStatorUnlock = false,
                    gearRatio = 1,
                    upshiftThreshold = 0,
                    downshiftThreshold = 0,
                }
            };

            pumpRpmFillCurve = new AnimationCurve()
            {
                preWrapMode = WrapMode.ClampForever,
                postWrapMode = WrapMode.ClampForever,
                keys = new[]
                {
                    new Keyframe(0, 0, 5e-5f, 3.57142853e-05f, 0, 0.333333343f),
                    new Keyframe(280, 0.01f, 5.9602713e-5f, 5.9602713e-5f, 0, 0.134199739f),
                    new Keyframe(350, 1, 0.0141428569f, 0, 0, 0),
                }
            };
        }

        public void ApplyDH4Defaults()
        {
            outputTorqueLimit = 1e9f;
            hydroDynamicBrakeTorqueCapacity = 50;
            couplingFillTime = 2;

            overheatingThreshold = 120;
            overheatingMaxTime = 5;
            overheatingDamagePerDegreePerSecond = 0.1f;
            overheatingExplosionDamage = 1200;

            configs = new[]
            {
                new HydraulicConfigDefinition()
                {
                    torqueCapacity = 0.5f,
                    stallTorqueMultiplier = 5.1f,
                    couplingSpeedRatio = 0.78f,
                    maxEfficiency = 0.86f,
                    hasStatorUnlock = false,
                    gearRatio = 4.5f,
                    upshiftThreshold = 0.12f,
                    downshiftThreshold = 0,
                },
                new HydraulicConfigDefinition()
                {
                    torqueCapacity = 1.5f,
                    stallTorqueMultiplier = 2.67f,
                    couplingSpeedRatio = 0.86f,
                    maxEfficiency = 0.86f,
                    hasStatorUnlock = false,
                    gearRatio = 4.5f,
                    upshiftThreshold = 0.18f,
                    downshiftThreshold = 0.11f,
                },
                new HydraulicConfigDefinition()
                {
                    torqueCapacity = 1.5f,
                    stallTorqueMultiplier = 2.67f,
                    couplingSpeedRatio = 0.86f,
                    maxEfficiency = 0.86f,
                    hasStatorUnlock = false,
                    gearRatio = 3.0f,
                    upshiftThreshold = 0,
                    downshiftThreshold = 0.17f,
                }
            };

            pumpRpmFillCurve = new AnimationCurve
            {
                preWrapMode = WrapMode.ClampForever,
                postWrapMode = WrapMode.ClampForever,
                keys = new[]
                {
                    new Keyframe(600, 0, 0, 0),
                    new Keyframe(700, 1, 0, 0),
                }
            };
        }

        #endregion


        [Serializable]
        public class HydraulicConfigDefinition
        {
            public float torqueCapacity;

            [Header("Max torque multiplier, applies at 0 speed ratio.")]
            public float stallTorqueMultiplier;

            [Header("Speed ratio where torque multiplier drops to 1.")]
            public float couplingSpeedRatio;

            [Header("Maximum achieved thermal efficiency. Must be >= couplingSpeedRatio and <= 1.")]
            public float maxEfficiency;
            public bool hasStatorUnlock;
            public float gearRatio;
            public float upshiftThreshold;
            public float downshiftThreshold;
        }
    }
}