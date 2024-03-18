using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation.Steam
{
    public class SteamCompressorDefinitionProxy : SimComponentDefinitionProxy, IS060Defaults, IS282Defaults
    {
        public float maxProductionRate = 250f;
        public float maxSteamConsumption = 1f;
        public float pressureForMaxProduction = 3f;
        public float activationPressureThreshold = 7f;
        public float mainReservoirVolume = 15f;
        public float smoothTime = 5f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "ACTIVATION_SIGNAL_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.PRESSURE, "MAIN_RES_PRESSURE_NORMALIZED"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "COMPRESSOR_HEALTH_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "STEAM_CONSUMPTION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "PRODUCTION_RATE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "PRODUCTION_RATE_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "MAIN_RES_VOLUME"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "ACTIVATION_PRESSURE_THRESHOLD"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "IS_POWERED")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "COMPRESSOR_CONTROL", false),
            new PortReferenceDefinition(DVPortValueType.PRESSURE, "STEAM_PRESSURE", false)
        };

        #region Defaults

        public void ApplyS060Defaults()
        {
            maxProductionRate = 20.0f;
            maxSteamConsumption = 1.0f;
            pressureForMaxProduction = 3.0f;
            activationPressureThreshold = 7.0f;
            mainReservoirVolume = 100.0f;
            smoothTime = 2.0f;
        }

        public void ApplyS282Defaults()
        {
            maxProductionRate = 20.0f;
            maxSteamConsumption = 1.0f;
            pressureForMaxProduction = 3.0f;
            activationPressureThreshold = 7.0f;
            mainReservoirVolume = 100.0f;
            smoothTime = 2.0f;
        }

        #endregion
    }
}
