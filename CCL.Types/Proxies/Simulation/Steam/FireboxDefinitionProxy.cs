using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Steam
{
    public class FireboxDefinitionProxy : SimComponentDefinitionProxy, IS060Defaults, IS282Defaults
    {
        [Header("Capacity")]
        public float maxCoalCapacity = 80f;
        public float coalDumpRate = 10f;

        [Header("Combustion")]
        public float burnTime = 120f;
        public float efficiencyAtMaxCombustion = 0.5f;
        public float combustionRateSmoothTime = 5f;
        public float temperatureSmoothTime = 15f;

        [Header("Fast Startup")]
        public float startupMaxCombustionMultiplier = 100f;
        public float startupMaxPressure = 13f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.COAL, "COAL_CAPACITY"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.COAL, "COAL_LEVEL"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "COAL_CONTROL_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.COAL, "COAL_ENV_DAMAGE_METER"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "IGNITION_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "EXTINGUISH_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.HEAT_RATE, "HEAT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "FIRE_ON"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.COAL, "COAL_DUMP_FLOW_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "SMOKE_DENSITY"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.COAL, "COMBUSTION_RATE_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TEMPERATURE, "TEMPERATURE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OXYGEN_AVAILABILITY")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "COAL_DUMP_CONTROL", false),
            new PortReferenceDefinition(DVPortValueType.STATE, "INTAKE_WATER_CONTENT", false),
            new PortReferenceDefinition(DVPortValueType.MASS_RATE, "AIR_FLOW", false),
            new PortReferenceDefinition(DVPortValueType.GENERIC, "FORWARD_SPEED", false),
            new PortReferenceDefinition(DVPortValueType.PRESSURE, "BOILER_PRESSURE", false),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "BOILER_TEMPERATURE", false),
            new PortReferenceDefinition(DVPortValueType.STATE, "BOILER_BROKEN_STATE", false)
        };

        #region Defaults

        public void ApplyS060Defaults()
        {
            maxCoalCapacity = 45.0f;
            coalDumpRate = 4.0f;

            burnTime = 120.0f;
            efficiencyAtMaxCombustion = 0.5f;
            combustionRateSmoothTime = 5.0f;
            temperatureSmoothTime = 15.0f;

            startupMaxCombustionMultiplier = 25.0f;
            startupMaxPressure = 13.0f;
        }

        public void ApplyS282Defaults()
        {
            maxCoalCapacity = 120.0f;
            coalDumpRate = 10.0f;

            burnTime = 120.0f;
            efficiencyAtMaxCombustion = 0.5f;
            combustionRateSmoothTime = 5.0f;
            temperatureSmoothTime = 15.0f;

            startupMaxCombustionMultiplier = 25.0f;
            startupMaxPressure = 13.0f;
        }

        #endregion
    }
}
