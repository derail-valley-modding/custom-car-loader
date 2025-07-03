using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Electric
{
    [AddComponentMenu("CCL/Proxies/Simulation/Electric/Slugs Power Calculator Definition Proxy")]
    public class SlugsPowerCalculatorDefinitionProxy : SimComponentDefinitionProxy
    {
        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.OHMS, "EXTERNAL_EFFECTIVE_RESISTANCE_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.AMPS, "EXTERNAL_AMPS_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.OHMS, "EFFECTIVE_RESISTANCE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.AMPS, "TOTAL_AMPS"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.OHMS, "INTERNAL_EFFECTIVE_RESISTANCE", false),
            new PortReferenceDefinition(DVPortValueType.AMPS, "INTERNAL_AMPS", false)
        };
    }
}
