using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class ConfigurableAddDefinitionProxy : SimComponentDefinitionProxy
    {
        [Tooltip("Leave as generic to show all options")]
        public DVPortValueType valueFilter = DVPortValueType.GENERIC;
        public bool negativeA;
        public bool negativeB;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, valueFilter, "ADD_OUT"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(valueFilter, "A"),
            new PortReferenceDefinition(valueFilter, "B"),
        };
    }
}
