using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Diesel
{
    public class DieselElectricGensetProxy : SimComponentDefinitionProxy
    {
        public float engineRpmMax = 1000f;
        public float engineRpmIdle = 120f;
        public float maxPower = 2200000f;

        public float fuelInjection = 5f;

        public float rpmGainFromFuel = 150f;
        public float rpmGainNoLoadMultiplier = 10f;
        public float rpmGainMinLoadMultiplier = 1f;
        public float rpmGainMaxLoadMultiplier = 0.5f;

        public float fuelConsumptionSmoothTime = 0.1f;
        public float oilConsumptionRate = 0.1f;

        public float ignitionTime = 0.5f;
        public float engineDragFalloff = 0.05f;

        [Header("Heat Management")]
        public float idleTemperature = 52f;
        public float heatRateFromRpm = 8f;
        public float heatRateBelowIdleFactor = 2f;
        public float overheatingTemperatureThreshold = 120f;
        public float overheatingMaxTime = 12f;

        [Header("Damage Properties")]
        public float noOilDamagePerSecond = 30f;
        public float rpmDamagePerSecond = 0.05f;
        public float overheatingDamagePerDegreePerSecond = 0.1f;
        public float damagePerIgnition = 10f;

        public float enginePerformanceDropHealthPercentage = 0.2f;
        public float damagedEnginePowerConstraintStart = 1f;
        public float damagedEnginePowerConstraintEnd = 0.2f;
        public float severeDamageEngineOffProbabilityMultiplier = 0.5f;

        [FuseId]
        public string engineStarterFuseId;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "IGNITION_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "EMERGENCY_ENGINE_OFF_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "COLLISION_ENGINE_OFF_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "ENGINE_HEALTH_STATE_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.FUEL, "FUEL_ENV_DAMAGE_METER"),
            new PortDefinition(DVPortType.OUT, DVPortValueType.POWER, "POWER_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.HEAT_RATE, "HEAT_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "RPM"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "RPM_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "IDLE_RPM_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "ENGINE_ON"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "IGNITION_IN_PROGESS"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "MAX_POWER"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "MAX_RPM"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "GOAL_POWER_SMOOTH_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.DAMAGE, "GENERATED_ENGINE_DAMAGE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.FUEL, "FUEL_CONSUMPTION_NORMALIZED"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.STATE, "INTERNAL_ENGINE_OFF"),
            new PortReferenceDefinition(DVPortValueType.FUEL, "FUEL"),
            new PortReferenceDefinition(DVPortValueType.FUEL, "FUEL_CONSUMPTION", writeAllowed: true),
            new PortReferenceDefinition(DVPortValueType.OIL, "OIL"),
            new PortReferenceDefinition(DVPortValueType.OIL, "OIL_CONSUMPTION", writeAllowed: true),
            new PortReferenceDefinition(DVPortValueType.POWER, "GOAL_POWER"),
            new PortReferenceDefinition(DVPortValueType.POWER, "SEC_GOAL_POWER"),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "LOAD_ON_ROTOR"),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TEMPERATURE"),
        };

        #region Defaults

        [MethodButton(nameof(ApplyDE2Defaults), "Apply DE2 Defaults")]
        [MethodButton(nameof(ApplyDE6Defaults), "Apply DE6 Defaults")]
        [RenderMethodButtons]
        public bool buttonRender;

        public void ApplyDE2Defaults()
        {
            engineRpmMax = 2000;
            engineRpmIdle = 700.0f;
            maxPower = 600000.0f;
            fuelInjection = 1.125f;
            rpmGainFromFuel = 1200.0f;
            rpmGainNoLoadMultiplier = 10.0f;
            rpmGainMinLoadMultiplier = 1.0f;
            rpmGainMaxLoadMultiplier = 0.4f;
            fuelConsumptionSmoothTime = 0.1f;
            oilConsumptionRate = 0.02f;
            ignitionTime = 1.0f;
            engineDragFalloff = 0.5f;
            idleTemperature = 52.0f;
            heatRateFromRpm = 0.0f;
            heatRateBelowIdleFactor = 0.0f;
            overheatingTemperatureThreshold = 105.0f;
            overheatingMaxTime = 12.0f;
            noOilDamagePerSecond = 30.0f;
            rpmDamagePerSecond = 0.05f;
            overheatingDamagePerDegreePerSecond = 0.1f;
            damagePerIgnition = 10.0f;
            enginePerformanceDropHealthPercentage = 0.2f;
            damagedEnginePowerConstraintStart = 1.0f;
            damagedEnginePowerConstraintEnd = 0.2f;
            severeDamageEngineOffProbabilityMultiplier = 0.5f;
        }

        public void ApplyDE6Defaults()
        {
            engineRpmMax = 1000.0f;
            engineRpmIdle = 200.0f;
            maxPower = 850000.0f;
            fuelInjection = 1.75f;
            rpmGainFromFuel = 400.0f;
            rpmGainNoLoadMultiplier = 2.0f;
            rpmGainMinLoadMultiplier = 1.0f;
            rpmGainMaxLoadMultiplier = 0.75f;
            fuelConsumptionSmoothTime = 0.1f;
            oilConsumptionRate = 0.1f;
            ignitionTime = 1.0f;
            engineDragFalloff = 0.6f;
            idleTemperature = 52.0f;
            heatRateFromRpm = 0.0f;
            heatRateBelowIdleFactor = 0.0f;
            overheatingTemperatureThreshold = 120.0f;
            overheatingMaxTime = 12.0f;
            noOilDamagePerSecond = 30.0f;
            rpmDamagePerSecond = 0.05f;
            overheatingDamagePerDegreePerSecond = 0.1f;
            damagePerIgnition = 10.0f;
            enginePerformanceDropHealthPercentage = 0.2f;
            damagedEnginePowerConstraintStart = 1.0f;
            damagedEnginePowerConstraintEnd = 0.2f;
            severeDamageEngineOffProbabilityMultiplier = 0.5f;
        }

        #endregion
    }
}