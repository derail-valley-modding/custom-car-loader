using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Steam
{
    public class BoilerDefinitionProxy : SimComponentDefinitionProxy, IS060Defaults, IS282Defaults
    {
        [Header("Dimensions")]
        public float diameter;
        public float length;
        public float capacityMultiplier;

        [Header("Injector")]
        public float maxInjectorRate;
        public float defaultFeedwaterTemperature = 110f;
        public float waterConsumptionMultiplier = 1f;

        [Header("Blowdown")]
        public float maxBlowdownRate = 10f;

        [Header("Safety valve")]
        public float safetyValveOpeningPressure;
        public float safetyValveClosingPressure;
        public float safetyValveSlop;
        public float maxSafetyValveVentRate;

        [Header("Spawn")]
        public float spawnPressure;
        public float spawnWaterLevel;

        [Header("Damage")]
        public float crownSheetNormalizedWaterLevel;
        public float crownSheetTempSmoothTime;
        public float crownSheetOverheatTemp;
        public float minimumExplosionPressure;
        public AnimationCurve explosionPressureThreshold;
        public float steamOutletNormalizedWaterLevel = 0.95f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, "BOILER_ANGLE_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.PRESSURE, "PRESSURE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TEMPERATURE, "TEMPERATURE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.WATER, "INJECTOR_FLOW_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.WATER, "BLOWDOWN_FLOW_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.WATER, "WATER_LEVEL_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.WATER, "WATER_MASS"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OUTLET_STEAM_QUALITY"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.WATER, "WATER_INSTANT_REMOVAL_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "SAFETY_VALVE_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TEMPERATURE, "CROWN_SHEET_TEMPERATURE_NORMALIZED"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "BODY_HEALTH_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "IS_BROKEN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "ENTHALPY"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "POWER_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "POWER_OUT")
    };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "INJECTOR", false),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "BLOWDOWN", false),
            new PortReferenceDefinition(DVPortValueType.HEAT_RATE, "HEAT", false),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "FIREBOX_TEMPERATURE", false),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "FEEDWATER_TEMPERATURE", false),
            new PortReferenceDefinition(DVPortValueType.MASS_RATE, "STEAM_CONSUMPTION", false),
            new PortReferenceDefinition(DVPortValueType.WATER, "WATER", false),
            new PortReferenceDefinition(DVPortValueType.WATER, "WATER_CONSUMPTION", true)
        };

        private static AnimationCurve ExplosionCurve => new AnimationCurve(
            new Keyframe(0.0f, 0.5f) { inTangent = 2.5f, outTangent = 2.5f },
            new Keyframe(1.0f, 3.0f) { inTangent = 2.5f, outTangent = 2.5f });

        #region Defaults

        public void ApplyS060Defaults()
        {
            diameter = 1.25f;
            length = 4.6f;
            capacityMultiplier = 0.85f;

            maxInjectorRate = 5.0f;
            defaultFeedwaterTemperature = 25.0f;
            waterConsumptionMultiplier = 2.0f;

            maxBlowdownRate = 5.0f;

            safetyValveOpeningPressure = 15.5f;
            safetyValveClosingPressure = 15.3f;
            safetyValveSlop = 0.1f;
            maxSafetyValveVentRate = 2.0f;

            spawnPressure = 1.0f;
            spawnWaterLevel = 3600.0f;

            crownSheetNormalizedWaterLevel = 0.65f;
            crownSheetTempSmoothTime = 60.0f;
            crownSheetOverheatTemp = 600.0f;
            minimumExplosionPressure = 5.0f;
            explosionPressureThreshold = ExplosionCurve;
            steamOutletNormalizedWaterLevel = 0.95f;
        }

        public void ApplyS282Defaults()
        {
            diameter = 1.7f;
            length = 8.8f;
            capacityMultiplier = 0.85f;

            maxInjectorRate = 20.0f;
            defaultFeedwaterTemperature = 110.0f;
            waterConsumptionMultiplier = 4.0f;

            maxBlowdownRate = 100.0f;

            safetyValveOpeningPressure = 15.0f;
            safetyValveClosingPressure = 14.8f;
            safetyValveSlop = 0.1f;
            maxSafetyValveVentRate = 5.0f;

            spawnPressure = 1.0f;
            spawnWaterLevel = 13000.0f;

            crownSheetNormalizedWaterLevel = 0.6f;
            crownSheetTempSmoothTime = 60.0f;
            crownSheetOverheatTemp = 600.0f;
            minimumExplosionPressure = 5.0f;
            explosionPressureThreshold = ExplosionCurve;
            steamOutletNormalizedWaterLevel = 0.98f;
        }

        #endregion
    }
}
