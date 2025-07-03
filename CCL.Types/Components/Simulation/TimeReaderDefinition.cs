using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Simulation
{
    [AddComponentMenu("CCL/Components/Simulation/Time Reader Definition")]
    public class TimeReaderDefinition : SimComponentDefinitionProxy
    {
        public override IEnumerable<PortDefinition> ExposedPorts => new[]
{
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "HOURS"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "MINUTES"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "NORMALIZED")
        };
    }
}
