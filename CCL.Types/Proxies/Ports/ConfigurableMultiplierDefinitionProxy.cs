using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class ConfigurableMultiplierDefinitionProxy : SimComponentDefinitionProxy
    {
        [Tooltip("Leave as generic to show all options")]
        public DVPortValueType valueFilter = DVPortValueType.GENERIC;
        public bool invertA;
        public bool invertB;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, valueFilter, "MUL_OUT"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(valueFilter, "A"),
            new PortReferenceDefinition(valueFilter, "B"),
        };
    }
}