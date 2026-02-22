using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    [AddComponentMenu("CCL/Proxies/Simulation/Automatic Transmission Input Definition Proxy")]
    public class AutomaticTransmissionInputDefinitionProxy : SimComponentDefinitionProxy
    {
        public float gearUpRpmThreshold = 800f;
        public float gearDownRpmThreshold = 120f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.CONTROL, "GEAR")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.RPM, "RPM_INDICATOR", false),
            new PortReferenceDefinition(DVPortValueType.GENERIC, "NUM_OF_GEARS", false)
        };
    }
}
