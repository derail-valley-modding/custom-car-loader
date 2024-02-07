using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class ConstantPortDefinitionProxy : SimComponentDefinitionProxy
    {
        public float value;
        public PortDefinition port = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, string.Empty);

        public override IEnumerable<PortDefinition> ExposedPorts => new[] { port };
    }
}