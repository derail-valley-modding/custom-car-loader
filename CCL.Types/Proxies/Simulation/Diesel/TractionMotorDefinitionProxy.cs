using CCL.Types.Json;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Wheels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Diesel
{
    public class TractionMotorDefinitionProxy : SimComponentDefinitionProxy, ICustomSerialized
    {
        public float maxGeneratorVoltage = 1200f;
        public float maxRpm = 650f;

        public int numberOfTractionMotors = 4;
        public float maxAmpsPerTractionMotor = 2000f;

        public float torqueLimitRpmPercentage = 0.1f;
        public float torqueFactor = 2f;

        public float oppositeDirectionTorquePercentage = 1f;

        [Header("Dynamic Braking")]
        public float dynamicBrakeMaxPowerPerTractionMotor = 275000f;
        public float dynamicBrakeVoltage = 600f;
        public float dynamicBrakeTorqueFactor = 1f;
        public float dynamicBrakeThrottlePercentage;
        public float dynamicBrakeMinEffectRpmPercentage = 0.06f;
        public float dynamicBrakeMaxEffectRpmPercentage = 0.8f;

        [Header("Transition")]
        public float contactor0To1SwitchDuration = 1f;
        public float contactor1To0DelayTime;

        public ContactorTransition[] contactorTransitions;
        [SerializeField, HideInInspector]
        private string? _transitionJson;

        [Header("Heat Management")]
        public float heatRateFromAmps = 8f;
        public float heatRateOverMaxAmpsFactor = 2f;
        public float heatRateDynamicBrakeFactor = 5f;
        public float heatRateOppositeDirectionSpeedFactor = 5f;

        public float overheatingTemperatureThreshold = 120f;
        public float overheatingMaxTime = 5f;
        public float overheatingTmKillPercentage = 0.2f;
        public float setTmOnFireOnKillPercentage = 0.3f;

        [Header("Damage Properties")]
        public float damagePerRpmOverMaxPerSecond = 0.1f;
        public float rpmDamagePerSecond = 0.05f;
        public float overheatingDamagePerDegreePerSecond = 0.1f;
        public float damagePerFuseBlow = 20f;
        public float damagePerTmBlow = 300f;

        public float tmPerformanceDropHealthPercentage = 0.2f;

        public float damagedTmTorqueConstraintStart = 1f;
        public float damagedTmTorqueConstraintEnd = 0.5f;

        public PoweredWheelsManagerProxy poweredWheelsManager;

        [FuseId]
        public string powerFuseId;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "HEALTH_STATE_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "WORKING_TRACTION_MOTORS"),
            new PortDefinition(DVPortType.IN, DVPortValueType.POWER, "POWER_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "GOAL_POWER"),
            new PortDefinition(DVPortType.OUT, DVPortValueType.TORQUE, "TORQUE_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.HEAT_RATE, "HEAT_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.AMPS, "AMPS"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.AMPS, "AMPS_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.AMPS, "MAX_AMPS"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, "GENERATOR_VOLTAGE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "RPM"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "RPM_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "DYNAMIC_BRAKE_EFFECT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.CONTROL, "LOAD_ON_GENERATOR"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CONTACTOR_0_TO_1"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CONTACTOR_1_TO_0"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CONTACTOR_TRANSITION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "OVERHEAT_POWER_FUSE_OFF"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "TMS_STATE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.DAMAGE, "GENERATED_ENGINE_DAMAGE"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, "EXTERNAL_TMS_NUM_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TORQUE, "EXTERNAL_TMS_TORQUE"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.POWER, "THROTTLE_POWER"),
            new PortReferenceDefinition(DVPortValueType.GENERIC, "THROTTLE_NOTCH"),
            new PortReferenceDefinition(DVPortValueType.GENERIC, "THROTTLE_PREV_NOTCH"),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "REVERSER"),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "DYNAMIC_BRAKE"),
            new PortReferenceDefinition(DVPortValueType.RPM, "WHEEL_RPM"),
            new PortReferenceDefinition(DVPortValueType.GENERIC, "GEAR_RATIO"),
            new PortReferenceDefinition(DVPortValueType.POWER, "MAX_POWER_PROVIDED"),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TEMPERATURE"),
        };

        public void OnValidate()
        {
            _transitionJson = JSONObject.ToJson(contactorTransitions);
        }

        public void AfterImport()
        {
            if (!string.IsNullOrWhiteSpace(_transitionJson))
            {
                contactorTransitions = JSONObject.FromJson<ContactorTransition[]>(_transitionJson);
            }
            else
            {
                contactorTransitions = new ContactorTransition[0];
            }
        }

        #region Defaults

        [MethodButton(nameof(ApplyDE2Defaults), "Apply DE2 Defaults")]
        [MethodButton(nameof(ApplyDE6Defaults), "Apply DE6 Defaults")]
        [RenderMethodButtons]
        public bool buttonRender;
        
        public void ApplyDE2Defaults()
        {
            maxGeneratorVoltage = 600.0f;
            maxRpm = 2200.0f;
            numberOfTractionMotors = 2;
            maxAmpsPerTractionMotor = 1000.0f;
            torqueLimitRpmPercentage = 0.1f;
            torqueFactor = 3.0f;
            oppositeDirectionTorquePercentage = 0.2f;
            dynamicBrakeMaxPowerPerTractionMotor = 0.0f;
            dynamicBrakeVoltage = 1.0f;
            dynamicBrakeTorqueFactor = 0.0f;
            dynamicBrakeThrottlePercentage = 0.0f;
            dynamicBrakeMinEffectRpmPercentage = 0.06f;
            dynamicBrakeMaxEffectRpmPercentage = 0.8f;
            contactor0To1SwitchDuration = 0.25f;
            contactor1To0DelayTime = 0.0f;
            contactorTransitions = Array.Empty<ContactorTransition>();
            heatRateFromAmps = 5.0f;
            heatRateOverMaxAmpsFactor = 2.0f;
            heatRateDynamicBrakeFactor = 0.0f;
            heatRateOppositeDirectionSpeedFactor = 10.0f;
            overheatingTemperatureThreshold = 120.0f;
            overheatingMaxTime = 2.0f;
            overheatingTmKillPercentage = 0.2f;
            setTmOnFireOnKillPercentage = 0.3f;
            damagePerRpmOverMaxPerSecond = 0.0f;
            rpmDamagePerSecond = 0.05f;
            overheatingDamagePerDegreePerSecond = 0.1f;
            damagePerFuseBlow = 20.0f;
            damagePerTmBlow = 300.0f;
            tmPerformanceDropHealthPercentage = 0.2f;
            damagedTmTorqueConstraintStart = 1.0f;
            damagedTmTorqueConstraintEnd = 0.5f;
        }

        public void ApplyDE6Defaults()
        {
            maxGeneratorVoltage = 800.0f;
            maxRpm = 3000.0f;
            numberOfTractionMotors = 6;
            maxAmpsPerTractionMotor = 600.0f;
            torqueLimitRpmPercentage = 0.1f;
            torqueFactor = 6.0f;
            oppositeDirectionTorquePercentage = 0.2f;
            dynamicBrakeMaxPowerPerTractionMotor = 275000.0f;
            dynamicBrakeVoltage = 400.0f;
            dynamicBrakeTorqueFactor = 7.0f;
            dynamicBrakeThrottlePercentage = 0.43f;
            dynamicBrakeMinEffectRpmPercentage = 0.06f;
            dynamicBrakeMaxEffectRpmPercentage = 0.8f;
            contactor0To1SwitchDuration = 2.0f;
            contactor1To0DelayTime = 0.0f;
            contactorTransitions = new ContactorTransition[]
            {
                new ContactorTransition()
                {
                    transitionSwitchTmRpm = 900,
                    transitionDuration = 1,
                    connectedToGen = false,
                },
                new ContactorTransition()
                {
                    transitionSwitchTmRpm = 1500,
                    transitionDuration = 1,
                    connectedToGen = false,
                }
            };
            heatRateFromAmps = 3.0f;
            heatRateOverMaxAmpsFactor = 1.25f;
            heatRateDynamicBrakeFactor = 0.1f;
            heatRateOppositeDirectionSpeedFactor = 20.0f;
            overheatingTemperatureThreshold = 120.0f;
            overheatingMaxTime = 2.0f;
            overheatingTmKillPercentage = 0.2f;
            setTmOnFireOnKillPercentage = 0.3f;
            damagePerRpmOverMaxPerSecond = 0.0f;
            rpmDamagePerSecond = 0.05f;
            overheatingDamagePerDegreePerSecond = 0.1f;
            damagePerFuseBlow = 20.0f;
            damagePerTmBlow = 300.0f;
            tmPerformanceDropHealthPercentage = 0.2f;
            damagedTmTorqueConstraintStart = 1.0f;
            damagedTmTorqueConstraintEnd = 0.5f;
        }

        #endregion

        [Serializable]
        public class ContactorTransition
        {
            public float transitionSwitchTmRpm;

            public float transitionDuration;

            public bool connectedToGen;
        }
    }
}
