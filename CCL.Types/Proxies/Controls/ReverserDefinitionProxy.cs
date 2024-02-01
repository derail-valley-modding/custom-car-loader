using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Controls
{
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
