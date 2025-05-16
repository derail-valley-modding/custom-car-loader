using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Electric
{
    public class TractionGeneratorDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields, IDE2Defaults, IDE6Defaults,
        IRecommendedDebugPorts
    {
        [Header("Generator")]
        public float maxVoltage = 1000f;
        public float torqueFactor = 1f;
        public float maxAmps = 4000f;

        [Header("Throttle Control")]
        public float throttleProportionalGain;
        public float throttleDifferentialGain;
        public float throttleIntegralGain;
        public float throttleIntegralMin;
        public float throttleIntegralMax;
        public float throttleMaxSpeed = 1f;
        public float excitationGainMaxSpeed = 1f;
        public float excitationDropMaxSpeed = 1f;

        [Header("Dynamic Brake")]
        public float dynamicBrakeGoalRpmNormalized = 0.5f;

        [FuseId]
        public string powerFuseId = string.Empty;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.CONTROL, "THROTTLE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TORQUE, "LOAD_TORQUE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, "VOLTAGE"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.AMPS, "EXTERNAL_CURRENT_LIMIT_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "EXTERNAL_CURRENT_LIMIT_ACTIVE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "OVERCURRENT_POWER_FUSE_OFF"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "EXCITATION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "POWER_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "POWER_OUT")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.POWER, "GOAL_POWER", false),
            new PortReferenceDefinition(DVPortValueType.RPM, "GOAL_RPM_NORMALIZED", false),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "DYNAMIC_BRAKE", false),
            new PortReferenceDefinition(DVPortValueType.RPM, "RPM", false),
            new PortReferenceDefinition(DVPortValueType.RPM, "RPM_NORMALIZED", false),
            new PortReferenceDefinition(DVPortValueType.AMPS, "TOTAL_AMPS", false),
            new PortReferenceDefinition(DVPortValueType.OHMS, "EFFECTIVE_RESISTANCE", false),
            new PortReferenceDefinition(DVPortValueType.OHMS, "SINGLE_MOTOR_EFFECTIVE_RESISTANCE", false),
            new PortReferenceDefinition(DVPortValueType.AMPS, "TRANSITION_CURRENT_LIMIT", false)
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(powerFuseId), powerFuseId),
        };

        public IEnumerable<string> GetDebugPorts() => new[]
        {
            "THROTTLE",
            "EXCITATION",
            "VOLTAGE",
            "POWER_IN"
        };

        [MethodButton(nameof(ApplyDE2Defaults), "Apply DE2 Defaults")]
        [MethodButton(nameof(ApplyDE6Defaults), "Apply DE6 Defaults")]
        [RenderMethodButtons]
        public bool buttonRender;

        #region Defaults

        public void ApplyDE2Defaults()
        {
            maxVoltage = 1600.0f;
            torqueFactor = 1.5f;
            maxAmps = 2500.0f;

            throttleProportionalGain = 5.0f;
            throttleDifferentialGain = 10.0f;
            throttleIntegralGain = 0.0f;
            throttleIntegralMin = 0.0f;
            throttleIntegralMax = 0.0f;
            throttleMaxSpeed = 1.0f;
            excitationGainMaxSpeed = 1.0f;
            excitationDropMaxSpeed = 1.0f;

            dynamicBrakeGoalRpmNormalized = 0.5f;
        }

        public void ApplyDE6Defaults()
        {
            maxVoltage = 600.0f;
            torqueFactor = 8.0f;
            maxAmps = 5000.0f;

            throttleProportionalGain = 5.0f;
            throttleDifferentialGain = 10.0f;
            throttleIntegralGain = 0.0f;
            throttleIntegralMin = 0.0f;
            throttleIntegralMax = 0.0f;
            throttleMaxSpeed = 1.0f;
            excitationGainMaxSpeed = 0.2f;
            excitationDropMaxSpeed = 1.0f;

            dynamicBrakeGoalRpmNormalized = 0.4578947f;
        }

        #endregion
    }
}
