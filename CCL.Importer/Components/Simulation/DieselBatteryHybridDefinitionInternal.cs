using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation
{
    internal class DieselBatteryHybridDefinitionInternal : SimComponentDefinition
    {
        public float modeTransitionDelay = 1f;
        public float maxChargeBatteryLevel = 0.95f;
        public float startChargeBatteryLevel = 0.6f;
        public float emergencyBatteryLevel = 0.4f;
        public float batteryRechargeRate = 200000f;
        public float engineRechargeThrottle = 0.5f;
        public float adaptiveChargingMaxPower = 0f;
        public bool manualChargingOnly = false;
        public bool adaptiveCharging = false;
        public bool adjustableChargeHysteresis = false;

        public string powerFuseId = string.Empty;

        public readonly PortReferenceDefinition throttle =
            new(PortValueType.CONTROL, "THROTTLE");
        public readonly PortReferenceDefinition generatorVoltage =
            new(PortValueType.VOLTS, "GENERATOR_VOLTS");
        public readonly PortReferenceDefinition voltRegVoltage =
            new(PortValueType.VOLTS, "VOLTAGE_REGULATOR_VOLTS");
        public readonly PortReferenceDefinition batteryVoltage =
            new(PortValueType.VOLTS, "BATTERY_VOLTS");
        public readonly PortReferenceDefinition batteryChargeNorm =
            new(PortValueType.ELECTRIC_CHARGE, "BATTERY_CHARGE_NORMALIZED");
        public readonly PortReferenceDefinition singleMotorEffectiveResistance =
            new(PortValueType.OHMS, "SINGLE_MOTOR_EFFECTIVE_RESISTANCE");
        public readonly PortReferenceDefinition effectiveResistance =
            new(PortValueType.OHMS, "EFFECTIVE_RESISTANCE");
        public readonly PortReferenceDefinition genGoalPower =
            new(PortValueType.POWER, "GOAL_POWER");
        public readonly PortReferenceDefinition genPowerOutRead =
            new(PortValueType.POWER, "GENERATOR_POWER_OUT_READ");
        public readonly PortReferenceDefinition tmTotalAmps =
            new(PortValueType.POWER, "TM_TOTAL_AMPS");
        public readonly PortReferenceDefinition transitionCurrentLimit =
            new(PortValueType.POWER, "TM_TRANSITION_CURRENT_LIMIT");
        public readonly PortReferenceDefinition tmPowerIn =
            new(PortValueType.POWER, "TM_POWER_IN_READ");
        public readonly PortReferenceDefinition chargeDisableOverride =
            new(PortValueType.CONTROL, "CHARGING_DISABLE_OVERRIDE");
        public readonly PortReferenceDefinition manualChargeToggle =
            new(PortValueType.CONTROL, "MANUAL_CHARGE_TOGGLE");
        public readonly PortReferenceDefinition chargeLimitUpperBound =
            new(PortValueType.CONTROL, "CHARGING_LIMIT_UPPER_BOUND");
        public readonly PortReferenceDefinition chargeLimitLowerBound =
            new(PortValueType.CONTROL, "CHARGING_LIMIT_LOWER_BOUND");
        public readonly PortReferenceDefinition hybridModeControl =
            new(PortValueType.CONTROL, "HYBRID_MODE_SELECTOR");
        public readonly PortReferenceDefinition extAmpLimitGen =
            new(PortValueType.AMPS, "EXTERNAL_CURRENT_LIMIT_OUT_GENERATOR", writeAllowed: true);
        public readonly PortReferenceDefinition extAmpLimitVoltReg =
            new(PortValueType.AMPS, "EXTERNAL_CURRENT_LIMIT_OUT_VOLTAGE_REGULATOR", writeAllowed: true);

        public readonly PortDefinition extCurrentLimitExtIn =
            new(PortType.EXTERNAL_IN, PortValueType.AMPS, "EXTERNAL_CURRENT_LIMIT_EXT_IN");
        public readonly PortDefinition throttleDeOut =
            new(PortType.READONLY_OUT, PortValueType.CONTROL, "THROTTLE_OUT_ENGINE");
        public readonly PortDefinition throttleBatteryOut =
            new(PortType.READONLY_OUT, PortValueType.CONTROL, "THROTTLE_OUT_BATTERY");
        public readonly PortDefinition SmerGen =
            new(PortType.READONLY_OUT, PortValueType.OHMS, "SINGLE_MOTOR_EFFECTIVE_RESISTANCE_OUT_GENERATOR");
        public readonly PortDefinition SmerBatt =
            new(PortType.READONLY_OUT, PortValueType.OHMS, "SINGLE_MOTOR_EFFECTIVE_RESISTANCE_OUT_BATTERY");
        public readonly PortDefinition effectiveResistanceOut =
            new(PortType.READONLY_OUT, PortValueType.OHMS, "EFFECTIVE_RESISTANCE_OUT");
        public readonly PortDefinition tmPowerInReadOut =
            new(PortType.READONLY_OUT, PortValueType.POWER, "TM_POWER_IN_READ_OUT");
        public readonly PortDefinition voltageBusOut =
            new(PortType.READONLY_OUT, PortValueType.VOLTS, "VOLTS_OUT");
        public readonly PortDefinition genGoalPowerOut =
            new(PortType.READONLY_OUT, PortValueType.POWER, "GEN_GOAL_PWR_OUT");
        public readonly PortDefinition tmTotalAmpsOut =
            new(PortType.READONLY_OUT, PortValueType.POWER, "TM_TOTAL_AMPS_OUT");
        public readonly PortDefinition transitionCurrentLimitOut =
            new(PortType.READONLY_OUT, PortValueType.POWER, "TM_TRANSITION_CURRENT_LIMIT_OUT");
        public readonly PortDefinition chargingActive =
            new(PortType.READONLY_OUT, PortValueType.STATE, "CHARGING_ACTIVE_STATE");
        public readonly PortDefinition activeMode =
            new(PortType.READONLY_OUT, PortValueType.STATE, "ACTIVE_MODE_STATE");
        public readonly PortDefinition dieselModeState =
            new(PortType.READONLY_OUT, PortValueType.STATE, "DIESEL_MODE_STATE");
        public readonly PortDefinition batteryModeState =
            new(PortType.READONLY_OUT, PortValueType.STATE, "BATTERY_MODE_STATE");
        public readonly PortDefinition batteryLow =
            new(PortType.READONLY_OUT, PortValueType.STATE, "BATTERY_LOW_STATE");

        public override SimComponent InstantiateImplementation()
        {
            return new DieselBatteryHybrid(this);
        }
    }
}
