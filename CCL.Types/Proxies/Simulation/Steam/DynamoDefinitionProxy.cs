using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation.Steam
{
    public class DynamoDefinitionProxy : SimComponentDefinitionProxy, IS060Defaults, IS282Defaults
    {
        public float minOperatingPressure = 2f;
        public float steamConsumption = 0.1f;
        public float smoothTime = 2f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "STEAM_CONSUMPTION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "DYNAMO_FLOW_NORMALIZED")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "CONTROL", false),
            new PortReferenceDefinition(DVPortValueType.PRESSURE, "STEAM_PRESSURE", false)
        };

        #region Defaults

        public void ApplyS060Defaults()
        {
            minOperatingPressure = 2.0f;
            steamConsumption = 0.1f;
            smoothTime = 2.0f;
        }

        public void ApplyS282Defaults()
        {
            minOperatingPressure = 2.0f;
            steamConsumption = 0.1f;
            smoothTime = 2.0f;
        }

        #endregion
    }
}
