using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Diesel
{
    public class DieselEngineDirectDefinitionProxy : SimComponentDefinitionProxy
    {
        [Header("RPM Range")]
        public float rotationalInertia;
        public float viscousDampingFactor;
        public float engineRpmMax;
        public float engineRpmIdle;

        [Header("Power & Torque")]
        public AnimationCurve rpmToPowerCurve;
        public float retarderBrakingTorque;

        [Header("Resource Consumption")]
        public float fuelInjection;
        public float oilConsumptionRate;

        [Header("Damage")]
        public float noOilDamagePerSecond = 30f;
        public float rpmDamagePerSecond = 0.05f;
        public float rpmDamageImmunityTime = 2f;
        public float overheatingThreshold = 110f;
        public float overheatingDamagePerDegreePerSecond = 0.1f;

        [FuseId]
        public string engineStarterFuseId;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "IGNITION_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "IGNITION_IN_PROGRESS"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "RPM"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.FUEL, "FUEL_ENV_DAMAGE_METER"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.HEAT_RATE, "HEAT_OUT"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "EMERGENCY_ENGINE_OFF_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "COLLISION_ENGINE_OFF_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "ENGINE_HEALTH_STATE_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.DAMAGE, "GENERATED_ENGINE_DAMAGE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "ENGINE_ON"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "RPM_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "IDLE_RPM_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "MAX_RPM"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "RETARDER_BRAKE_EFFECT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.FUEL, "FUEL_CONSUMPTION_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "IS_BROKEN"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "THROTTLE"),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "RETARDER"),
            new PortReferenceDefinition(DVPortValueType.RPM, "DRIVEN_RPM"),
            new PortReferenceDefinition(DVPortValueType.FUEL, "FUEL"),
            new PortReferenceDefinition(DVPortValueType.FUEL, "FUEL_CONSUMPTION", true),
            new PortReferenceDefinition(DVPortValueType.OIL, "OIL"),
            new PortReferenceDefinition(DVPortValueType.OIL, "OIL_CONSUMPTION", true),
            new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE"),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TEMPERATURE"),
        };

        [MethodButton(nameof(ApplyDM3Defaults), "Apply DM3 Defaults")]
        [RenderMethodButtons]
        public bool buttonRender;

        #region Defaults

        public void ApplyDM3Defaults()
        {
            rotationalInertia = 3.0f;
            viscousDampingFactor = 20.0f;
            engineRpmMax = 1000.0f;
            engineRpmIdle = 300.0f;
            retarderBrakingTorque = 6500.0f;
            fuelInjection = 0.5f;
            oilConsumptionRate = 0.01f;
            noOilDamagePerSecond = 30.0f;
            rpmDamagePerSecond = 0.03f;
            rpmDamageImmunityTime = 2.0f;
            overheatingThreshold = 110.0f;
            overheatingDamagePerDegreePerSecond = 0.1f;

            rpmToPowerCurve = new AnimationCurve()
            {
                preWrapMode = WrapMode.ClampForever,
                postWrapMode = WrapMode.ClampForever,
                keys = new[]
                {
                    new Keyframe
                    {
                        time = 0.0f,
                        value = 0.0f,
                        inTangent = 1999.44836f,
                        inWeight = 0.0f,
                        outTangent = 1999.44836f,
                        outWeight = 0.213289589f,
                    },
                    new Keyframe
                    {
                        time = 100.0f,
                        value = 200000.0f,
                        inTangent = 645.608948f,
                        inWeight = 0.335520923f,
                        outTangent = 645.608948f,
                        outWeight = 0.03213099f,
                    },
                    new Keyframe
                    {
                        time = 840.0f,
                        value = 360000.0f,
                        inTangent = 0.09395475f,
                        inWeight = 0.07847808f,
                        outTangent = 0.09395475f,
                        outWeight = 0.7684883f,
                    },
                    new Keyframe
                    {
                        time = 950.0f,
                        value = 234000.0f,
                        inTangent = -3051.99121f,
                        inWeight = 0.116008788f,
                        outTangent = -3051.99121f,
                        outWeight = 0.179655761f,
                    },
                    new Keyframe
                    {
                        time = 1000.0f,
                        value = 10000.0f,
                        inTangent = -4480.0f,
                        inWeight = 0.333333343f,
                        outTangent = -4480.0f,
                        outWeight = 0.333333343f,
                    }
                }
            };
        }

        #endregion
    }
}
