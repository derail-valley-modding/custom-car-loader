using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Wheels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Electric
{
    public class TractionMotorSetDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields, ICustomSerialized, IDE2Defaults, IDE6Defaults
    {
        [Header("Motor")]
        public float maxMotorRpm;
        public float motorResistance = 0.015f;
        public float motorTorqueFactor = 1f;
        public float externalResistance = 0.01f;
        public float ampsSmoothTime = 0.5f;
        public float ampsSmoothMaxSpeed = 1000f;
        public int numberOfTractionMotors = 2;

        [Header("Dynamic Brake")]
        public float dynamicBrakePeakForceRpm = 1300f;
        public float dynamicBrakeGridResistance = 1f;
        public float dynamicBrakeMaxCurrent = 700f;
        public float dynamicBrakeCoolerSmoothTime = 5f;

        [Header("Transition")]
        public int circuitConnectionStages = 1;
        public float circuitConnectionTime = 1f;
        public float circuitConnectionTimeRandomization = 0.1f;
        public float transitionMaxAmps = 10f;
        public ElectricalConfigurationDefinition[] configurations = new ElectricalConfigurationDefinition[0];

        [Header("Damage")]
        public float overheatingTemperatureThreshold = 120f;
        public float overheatingMaxTime = 5f;
        public float overheatingTmKillPercentage = 0.2f;
        public float setTmOnFireOnKillPercentage = 0.3f;
        public float rpmDamagePerSecond = 0.05f;
        public float overspeedDamageFactor = 0.01f;
        public float overheatingDamagePerSecond = 0.1f;
        public float damagePerFuseBlow = 20f;
        public float damagePerTmBlow = 300f;
        public PoweredWheelsManagerProxy poweredWheelsManager = null!;

        [FuseId]
        public string powerFuseId;

        [SerializeField, HideInInspector]
        private string? _configs;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.OUT, DVPortValueType.TORQUE, "TORQUE_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.OHMS, "EFFECTIVE_RESISTANCE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.AMPS, "TOTAL_AMPS"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.AMPS, "TOTAL_AMPS_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.AMPS, "AMPS_PER_TM"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "RPM_NORMALIZED"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "HEALTH_STATE_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "WORKING_TRACTION_MOTORS"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.HEAT_RATE, "HEAT_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CONTACTOR_CHANGE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CURRENT_DROP_REQUEST"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "DYNAMIC_BRAKE_ACTIVE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "OVERHEAT_POWER_FUSE_OFF"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "TMS_STATE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "OVERSPEED_SOUND"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "OVERSPEED_EXPLOSION_TRIGGER"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.DAMAGE, "GENERATED_DAMAGE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "FIELD_FLUX"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, "MOTOR_VOLTS"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "ACTIVE_CONFIGURATION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "PENDING_CONFIGURATION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "POWER_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "POWER_OUT")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "THROTTLE", false),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "REVERSER", false),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "DYNAMIC_BRAKE", false),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "CONFIGURATION_OVERRIDE", false),
            new PortReferenceDefinition(DVPortValueType.RPM, "MOTOR_RPM", false),
            new PortReferenceDefinition(DVPortValueType.VOLTS, "APPLIED_VOLTAGE", false),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TM_TEMPERATURE", false)
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(powerFuseId), powerFuseId),
        };

        [MethodButton(nameof(ApplyDE2Defaults), "Apply DE2 Defaults")]
        [RenderMethodButtons]
        public bool buttonRender;

        [Serializable]
        public class ElectricalConfigurationDefinition
        {
            public float excitationMultiplier;
            public MotorGroupDefinition[] motorGroups = new MotorGroupDefinition[0];
            public TransitionDefinition forwardTransition = new TransitionDefinition();
            public TransitionDefinition backwardTransition = new TransitionDefinition();
        }

        [Serializable]
        public class MotorGroupDefinition
        {
            public int[] motorIndexes = new int[0];
        }

        [Serializable]
        public class TransitionDefinition
        {
            public float thresholdValue;
            public ThresholdType thresholdType;

            public enum ThresholdType
            {
                TRANSITION_WHEN_ABOVE_THRESHOLD,
                TRANSITION_WHEN_BELOW_THRESHOLD
            }
        }

        public void OnValidate()
        {
            _configs = JSONObject.ToJson(configurations);
        }

        public void AfterImport()
        {
            configurations = JSONObject.FromJson(_configs, () => configurations);
        }

        #region Defaults

        public void ApplyDE2Defaults()
        {
            maxMotorRpm = 2200.0f;
            motorResistance = 0.015f;
            motorTorqueFactor = 0.008f;
            externalResistance = 0.01f;
            ampsSmoothTime = 0.5f;
            ampsSmoothMaxSpeed = 5000.0f;
            numberOfTractionMotors = 2;

            dynamicBrakePeakForceRpm = 1300.0f;
            dynamicBrakeGridResistance = 1.0f;
            dynamicBrakeMaxCurrent = 700.0f;
            dynamicBrakeCoolerSmoothTime = 5.0f;

            circuitConnectionStages = 1;
            circuitConnectionTime = 0.2f;
            circuitConnectionTimeRandomization = 0.1f;
            transitionMaxAmps = 10.0f;
            configurations = new[]
            {
                new ElectricalConfigurationDefinition()
                {
                    excitationMultiplier = 1.0f,
                    motorGroups = new[]
                    {
                        new MotorGroupDefinition()
                        {
                            motorIndexes = new[] { 0 }
                        },
                        new MotorGroupDefinition()
                        {
                            motorIndexes = new[] { 1 }
                        }
                    },
                    forwardTransition = new TransitionDefinition()
                    {
                        thresholdValue = 0.0f,
                        thresholdType = TransitionDefinition.ThresholdType.TRANSITION_WHEN_ABOVE_THRESHOLD
                    },
                    backwardTransition = new TransitionDefinition()
                    {
                        thresholdValue = 0.0f,
                        thresholdType = TransitionDefinition.ThresholdType.TRANSITION_WHEN_ABOVE_THRESHOLD
                    }
                }
            };

            overheatingTemperatureThreshold = 120.0f;
            overheatingMaxTime = 5.0f;
            overheatingTmKillPercentage = 0.2f;
            setTmOnFireOnKillPercentage = 0.3f;
            rpmDamagePerSecond = 0.005f;
            overspeedDamageFactor = 0.001f;
            overheatingDamagePerSecond = 0.1f;
            damagePerFuseBlow = 20.0f;
            damagePerTmBlow = 300.0f;
        }

        public void ApplyDE6Defaults()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
