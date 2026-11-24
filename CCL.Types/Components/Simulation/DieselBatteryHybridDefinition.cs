using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Simulation
{
    [AddComponentMenu("CCL/Components/Simulation/Diesel Battery Hybrid Definition")]
    public class DieselBatteryHybridDefinition : SimComponentDefinitionProxy
    {
        [Tooltip("Mode transition delay in seconds")]
        public float modeTransitionDelay = 1f;
        [Tooltip("Battery charge level to stop charging")]
        public float maxChargeBatteryLevel = 0.95f;
        [Tooltip("Battery charge level to start charging")]
        public float startChargeBatteryLevel = 0.6f;
        [Tooltip("Battery charge level to switch to diesel mode")]
        public float emergencyBatteryLevel = 0.4f;
        [Tooltip("How much power the generator should make to charge the battery, in watts")]
        public float batteryRechargeRate = 200000f;
        [Tooltip("How much the engine is throttled while charging battery")]
        public float engineRechargeThrottle = 0.5f;
        [Tooltip("The maximum charging power drawn from the generator for adaptive charging, in watts. This value should not exceed the engine power output at the set recharge throttle RPM")]
        public float adaptiveChargingMaxPower = 0f;
        [Tooltip("Disables battery SoC hysteresis and only charge on manual control")]
        public bool manualChargingOnly = false;
        [Tooltip("Increases charging rate to maintain battery charging under traction load")]
        public bool adaptiveCharging = false;
        [Tooltip("Enable adjusting battery SoC hysteresis thresholds on the fly")]
        public bool adjustableChargeHysteresis = false;

        [FuseId]
        public string powerFuseId = string.Empty;

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "THROTTLE"),
            new PortReferenceDefinition(DVPortValueType.VOLTS, "GENERATOR_VOLTS"),
            new PortReferenceDefinition(DVPortValueType.VOLTS, "VOLTAGE_REGULATOR_VOLTS"),
            new PortReferenceDefinition(DVPortValueType.VOLTS, "BATTERY_VOLTS"),
            new PortReferenceDefinition(DVPortValueType.ELECTRIC_CHARGE, "BATTERY_CHARGE_NORMALIZED"),
            new PortReferenceDefinition(DVPortValueType.OHMS, "SINGLE_MOTOR_EFFECTIVE_RESISTANCE"),
            new PortReferenceDefinition(DVPortValueType.OHMS, "EFFECTIVE_RESISTANCE"),
            new PortReferenceDefinition(DVPortValueType.POWER, "GOAL_POWER"),
            new PortReferenceDefinition(DVPortValueType.POWER, "GENERATOR_POWER_OUT_READ"),
            new PortReferenceDefinition(DVPortValueType.AMPS, "TM_TOTAL_AMPS"),
            new PortReferenceDefinition(DVPortValueType.AMPS, "TM_TRANSITION_CURRENT_LIMIT"),
            new PortReferenceDefinition(DVPortValueType.POWER, "TM_POWER_IN_READ"),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "CHARGING_DISABLE_OVERRIDE"),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "MANUAL_CHARGE_TOGGLE"),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "CHARGING_LIMIT_UPPER_BOUND"),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "CHARGING_LIMIT_LOWER_BOUND"),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "HYBRID_MODE_SELECTOR"),
            new PortReferenceDefinition(DVPortValueType.AMPS, "EXTERNAL_CURRENT_LIMIT_OUT_GENERATOR", writeAllowed: true),
            new PortReferenceDefinition(DVPortValueType.AMPS, "EXTERNAL_CURRENT_LIMIT_OUT_VOLTAGE_REGULATOR", writeAllowed: true)
        };

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.AMPS, "EXTERNAL_CURRENT_LIMIT_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.CONTROL, "THROTTLE_OUT_ENGINE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.CONTROL, "THROTTLE_OUT_BATTERY"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.OHMS, "SINGLE_MOTOR_EFFECTIVE_RESISTANCE_OUT_GENERATOR"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.OHMS, "SINGLE_MOTOR_EFFECTIVE_RESISTANCE_OUT_BATTERY"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.OHMS, "EFFECTIVE_RESISTANCE_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "TM_POWER_IN_READ_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, "VOLTS_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "GEN_GOAL_PWR_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.AMPS, "TM_TOTAL_AMPS_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.AMPS, "TM_TRANSITION_CURRENT_LIMIT_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CHARGING_ACTIVE_STATE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "ACTIVE_MODE_STATE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "DIESEL_MODE_STATE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "BATTERY_MODE_STATE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "BATTERY_LOW_STATE")
        };
    }
}
