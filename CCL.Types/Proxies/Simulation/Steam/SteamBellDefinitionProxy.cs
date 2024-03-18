using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation.Steam
{
    public class SteamBellDefinitionProxy : SimComponentDefinitionProxy, IS060Defaults, IS282Defaults
    {
        public float steamConsumption = 0.1f;
        public float minOperatingPressure = 2f;
        public float smoothDownTime = 2f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "STEAM_CONSUMPTION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "BELL_NORMALIZED")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "CONTROL", false),
            new PortReferenceDefinition(DVPortValueType.PRESSURE, "STEAM_PRESSURE", false)
        };

        #region

        public void ApplyS060Defaults()
        {
            steamConsumption = 0.05f;
            minOperatingPressure = 2.0f;
            smoothDownTime = 1.0f;
        }

        public void ApplyS282Defaults()
        {
            steamConsumption = 0.05f;
            minOperatingPressure = 2.0f;
            smoothDownTime = 1.0f;
        }

        #endregion
    }
}
