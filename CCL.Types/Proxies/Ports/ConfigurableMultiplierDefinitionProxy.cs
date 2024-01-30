using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class ConfigurableMultiplierDefinitionProxy : SimComponentDefinitionProxy
    {
        [Header("Leave as generic to show all options")]
        public PortReferenceDefinition aReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "A");
        public PortReferenceDefinition bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "B");
        public PortDefinition mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "MUL_OUT");

        public bool invertA;
        public bool invertB;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            mulReadOut,
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            aReader,
            bReader,
        };
    }
}