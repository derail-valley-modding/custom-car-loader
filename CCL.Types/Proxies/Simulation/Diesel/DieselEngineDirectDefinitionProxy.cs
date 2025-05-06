using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Diesel
{
    public class DieselEngineDirectDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields,
        IDE2Defaults, IDE6Defaults, IDH4Defaults, IDM3Defaults, IDM1UDefaults,
        IRecommendedDebugPorts, IRecommendedDebugPortReferences
    {
        [Header("RPM Range")]
        public float rotationalInertia;
        public float viscousDampingFactor;
        public float engineRpmMax;
        public float engineRpmIdle;

        [Header("Power & Torque")]
        // Null :pensive:
        public AnimationCurve rpmToPowerCurve = null!;
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
        public string engineStarterFuseId = string.Empty;

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
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.DAMAGE, "GENERATED_ENGINE_PERCENTUAL_DAMAGE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "ENGINE_ON"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "RPM_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "IDLE_RPM_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "MAX_POWER_RPM_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "MAX_POWER"),
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
            new PortReferenceDefinition(DVPortValueType.STATE, "INTAKE_WATER_CONTENT"),
            new PortReferenceDefinition(DVPortValueType.FUEL, "FUEL"),
            new PortReferenceDefinition(DVPortValueType.FUEL, "FUEL_CONSUMPTION", true),
            new PortReferenceDefinition(DVPortValueType.OIL, "OIL"),
            new PortReferenceDefinition(DVPortValueType.OIL, "OIL_CONSUMPTION", true),
            new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE"),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TEMPERATURE"),
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(engineStarterFuseId), engineStarterFuseId),
        };

        public IEnumerable<string> GetDebugPorts() => new[]
        {
            "RPM"
        };

        public IEnumerable<string> GetDebugPortReferences() => new[]
        {
            "FUEL_CONSUMPTION"
        };

        #region Defaults

        public void ApplyDE2Defaults()
        {
            rotationalInertia = 5.0f;
            viscousDampingFactor = 20.0f;
            engineRpmMax = 2000.0f;
            engineRpmIdle = 600.0f;

            retarderBrakingTorque = 0.0f;

            fuelInjection = 0.3f;
            oilConsumptionRate = 0.02f;

            noOilDamagePerSecond = 30.0f;
            rpmDamagePerSecond = 0.005f;
            rpmDamageImmunityTime = 2.0f;
            overheatingThreshold = 110.0f;
            overheatingDamagePerDegreePerSecond = 0.1f;

            rpmToPowerCurve = new AnimationCurve()
            {
                preWrapMode = WrapMode.Loop,
                postWrapMode = WrapMode.Loop,
                keys = new[]
                {
                    new Keyframe
                    {
                        time = 0.0f,
                        value = 0.0f,
                        inTangent = 0.0f,
                        outTangent = 0.0f,
                        inWeight = 1 / 3f,
                        outWeight = 1 / 3f,
                    },
                    new Keyframe
                    {
                        time = 600.0f,
                        value = 100000.0f,
                        inTangent = 2000 / 9f,
                        outTangent = 2000 / 9f,
                        inWeight = 1 / 3f,
                        outWeight = 1 / 3f,
                    },
                    new Keyframe
                    {
                        time = 1800.0f,
                        value = 400000.0f,
                        inTangent = 0.0f,
                        outTangent = 0.0f,
                        inWeight = 1 / 3f,
                        outWeight = 1 / 3f,
                    },
                    new Keyframe
                    {
                        time = 2000.0f,
                        value = 0.0f,
                        inTangent = -4306.064f,
                        outTangent = -4306.064f,
                        inWeight = 1 / 3f,
                        outWeight = 1 / 3f,
                    }
                }
            };
        }

        public void ApplyDE6Defaults()
        {
            rotationalInertia = 50.0f;
            viscousDampingFactor = 100.0f;
            engineRpmMax = 950.0f;
            engineRpmIdle = 315.0f;

            retarderBrakingTorque = 0.0f;

            fuelInjection = 1.6f;
            oilConsumptionRate = 0.05f;

            noOilDamagePerSecond = 30.0f;
            rpmDamagePerSecond = 0.015f;
            rpmDamageImmunityTime = 2.0f;
            overheatingThreshold = 110.0f;
            overheatingDamagePerDegreePerSecond = 0.1f;

            rpmToPowerCurve = new AnimationCurve()
            {
                preWrapMode = WrapMode.Loop,
                postWrapMode = WrapMode.Loop,
                keys = new[]
                {
                    new Keyframe
                    {
                        time = 0.0f,
                        value = 0.0f,
                        inTangent = 1440.5848f,
                        outTangent = 1440.5848f,
                        inWeight = 1 / 3f,
                        outWeight = 0.10329206f,
                    },
                    new Keyframe
                    {
                        time = 275.0f,
                        value = 300000.0f,
                        inTangent = 1494.04f,
                        outTangent = 1494.04f,
                        inWeight = 1 / 3f,
                        outWeight = 0.04434629f,
                    },
                    new Keyframe
                    {
                        time = 900.0f,
                        value = 1700000.0f,
                        inTangent = 0.0f,
                        outTangent = 0.0f,
                        inWeight = 1 / 3f,
                        outWeight = 1 / 3f,
                    },
                    new Keyframe
                    {
                        time = 950.0f,
                        value = 0.0f,
                        inTangent = -68105.2f,
                        outTangent = -68105.2f,
                        inWeight = 0.035001222f,
                        outWeight = 1 / 3f,
                    }
                }
            };
        }

        public void ApplyDH4Defaults()
        {
            rotationalInertia = 10.0f;
            viscousDampingFactor = 40.0f;
            engineRpmMax = 1600.0f;
            engineRpmIdle = 600.0f;

            retarderBrakingTorque = 100000.0f;

            fuelInjection = 2.0f;
            oilConsumptionRate = 0.025f;

            noOilDamagePerSecond = 30.0f;
            rpmDamagePerSecond = 0.01f;
            rpmDamageImmunityTime = 2.0f;
            overheatingThreshold = 110.0f;
            overheatingDamagePerDegreePerSecond = 0.1f;

            rpmToPowerCurve = new AnimationCurve()
            {
                preWrapMode = WrapMode.Loop,
                postWrapMode = WrapMode.Loop,
                keys = new[]
                {
                    new Keyframe
                    {
                        time = 0.0f,
                        value = 0.0f,
                        inTangent = 842.26666f,
                        outTangent = 842.26666f,
                        inWeight = 1 / 3f,
                        outWeight = 0.031815805f,
                    },
                    new Keyframe
                    {
                        time = 1500.0f,
                        value = 1000000.0f,
                        inTangent = 0.000969854f,
                        outTangent = 0.000969854f,
                        inWeight = 1 / 3f,
                        outWeight = 1.0f,
                    },
                    new Keyframe
                    {
                        time = 1600.0f,
                        value = 0.0f,
                        inTangent = -27790.76f,
                        outTangent = -27790.76f,
                        inWeight = 0.062794186f,
                        outWeight = 1 / 3f,
                    }
                }
            };
        }

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

        public void ApplyDM1UDefaults()
        {
            rotationalInertia = 2.0f;
            viscousDampingFactor = 10.0f;
            engineRpmMax = 2200.0f;
            engineRpmIdle = 700.0f;

            retarderBrakingTorque = 6500.0f;

            fuelInjection = 0.1f;
            oilConsumptionRate = 0.001f;

            noOilDamagePerSecond = 30.0f;
            rpmDamagePerSecond = 0.006f;
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
                        inTangent = 73.12201f,
                        outTangent = 73.12201f,
                        inWeight = 0.0f,
                        outWeight = 0.030582821f,
                    },
                    new Keyframe
                    {
                        time = 1900.0f,
                        value = 110000.0f,
                        inTangent = 0.09395475f,
                        outTangent = 0.09395475f,
                        inWeight = 0.07847808f,
                        outWeight = 0.7684883f,
                    },
                    new Keyframe
                    {
                        time = 2500.0f,
                        value = 0.0f,
                        inTangent = -550 / 3f,
                        outTangent = -550 / 3f,
                        inWeight = 1 / 3f,
                        outWeight = 1 / 3f,
                    }
                }
            };
        }

        #endregion
    }
}
