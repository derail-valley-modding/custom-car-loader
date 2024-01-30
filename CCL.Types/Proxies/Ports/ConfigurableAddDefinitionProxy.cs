using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class ConfigurableAddDefinitionProxy : SimComponentDefinitionProxy
    {
        [Header("Leave as generic to show all options")]
        public PortReferenceDefinition aReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "A");
        public PortReferenceDefinition bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "B");
        public PortDefinition addReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "ADD_OUT");
        
        public bool negativeA;
        public bool negativeB;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            addReadOut,
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            aReader,
            bReader,
        };
    }
}
