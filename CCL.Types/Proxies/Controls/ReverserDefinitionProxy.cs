using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Reverser Definition Proxy")]
    public class ReverserDefinitionProxy : SimComponentDefinitionProxy
    {
        public bool isAnalog;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "CONTROL_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.CONTROL, "REVERSER"),
        };
    }
}
