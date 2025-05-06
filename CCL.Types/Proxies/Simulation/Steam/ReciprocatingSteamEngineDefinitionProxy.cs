using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Steam
{
    public class ReciprocatingSteamEngineDefinitionProxy : SimComponentDefinitionProxy, IS060Defaults, IS282Defaults,
        IRecommendedDebugPorts, IRecommendedDebugPortReferences
    {
        public int numCylinders = 2;
        public float cylinderBore = 0.533f;
        public float pistonStroke = 0.711f;
        public float minCutoff = 0.05f;
        public float maxCutoff = 0.9f;

        [Header("Throttle and steam chest")]
        public float throttleMaxFlow;
        public float steamChestVolume;

        [Header("Water in cylinders")]
        public float cylinderHeatRate = 0.05f;
        public float maxCondensationRate = 0.015f;
        public float primingRate = 0.05f;
        public float maxWaterExpulsionRate = 0.015f;
        public float waterDrainPercentagePerSecond = 0.3f;
        public float waterDamagePercentagePerChuff = 0.025f;
        public float waterDamageRestorePerSecond = 0.03f;

        [Header("Damage")]
        public float cylinderCrackDamage = 100f;
        public float passiveDamagePerRev = 0.01f;
        public float noOilDamagePerRev = 1f;
        public float ashChuffDamage = 1f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CRANK_ROTATION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CYLINDERS_INLET_VALVE_OPEN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "DUMPED_FLOW"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "MAX_FLOW"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "INTAKE_FLOW"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "INTAKE_FLOW_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "EXHAUST_FLOW"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.PRESSURE, "EXHAUST_PRESSURE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TEMPERATURE, "EXHAUST_TEMPERATURE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CHUFF_EVENT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CHUFF_FREQUENCY"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TEMPERATURE, "CYLINDER_TEMPERATURE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "WATER_IN_CYLINDERS_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "ASHES_IN_PIPES"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CYLINDER_COCK_FLOW_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "CYLINDER_CRACK_FLOW_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.PRESSURE, "STEAM_CHEST_PRESSURE"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "HEALTH_STATE_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.DAMAGE, "GENERATED_MECHANICAL_DAMAGE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "IS_CYLINDER_CRACKED"),
            new PortDefinition(DVPortType.OUT, DVPortValueType.TORQUE, "TORQUE_OUT")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "THROTTLE_CONTROL", false),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "REVERSER_CONTROL", false),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "CYLINDER_COCK_CONTROL", false),
            new PortReferenceDefinition(DVPortValueType.PRESSURE, "INTAKE_PRESSURE", false),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "INTAKE_TEMPERATURE", false),
            new PortReferenceDefinition(DVPortValueType.GENERIC, "INTAKE_QUALITY", false),
            new PortReferenceDefinition(DVPortValueType.RPM, "CRANK_RPM", false),
            new PortReferenceDefinition(DVPortValueType.OIL, "LUBRICATION_NORMALIZED", false)
        };

        public IEnumerable<string> GetDebugPorts() => new[]
        {
            "STEAM_CHEST_PRESSURE",
            "TORQUE_OUT"
        };

        public IEnumerable<string> GetDebugPortReferences() => new[]
        {
            "CRANK_RPM"
        };

        #region Defaults

        public void ApplyS060Defaults()
        {
            numCylinders = 2;
            cylinderBore = 0.42f;
            pistonStroke = 0.61f;
            minCutoff = 0.05f;
            maxCutoff = 0.9f;

            throttleMaxFlow = 3f;
            steamChestVolume = 200f;

            cylinderHeatRate = 0.01f;
            maxCondensationRate = 1.0f/300.0f;
            primingRate = 0.05f;
            maxWaterExpulsionRate = 0.015f;
            waterDrainPercentagePerSecond = 0.6f;
            waterDamagePercentagePerChuff = 0.025f;
            waterDamageRestorePerSecond = 0.03f;

            cylinderCrackDamage = 100f;
            passiveDamagePerRev = 0.01f;
            noOilDamagePerRev = 1f;
            ashChuffDamage = 3f;
        }

        public void ApplyS282Defaults()
        {
            numCylinders = 2;
            cylinderBore = 0.61f;
            pistonStroke = 0.711f;
            minCutoff = 0.05f;
            maxCutoff = 0.9f;

            throttleMaxFlow = 8f;
            steamChestVolume = 1000;

            cylinderHeatRate = 0.01f;
            maxCondensationRate = 0.015f;
            primingRate = 0.05f;
            maxWaterExpulsionRate = 0.015f;
            waterDrainPercentagePerSecond = 0.6f;
            waterDamagePercentagePerChuff = 0.025f;
            waterDamageRestorePerSecond = 0.03f;

            cylinderCrackDamage = 100f;
            passiveDamagePerRev = 0.01f;
            noOilDamagePerRev = 1f;
            ashChuffDamage = 3f;
        }

        #endregion
    }
}
