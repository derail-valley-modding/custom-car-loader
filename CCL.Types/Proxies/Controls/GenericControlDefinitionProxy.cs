using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Controls
{
    public class GenericControlDefinitionProxy : SimComponentDefinitionProxy
    {
        public float defaultValue;
        public float smoothTime;
        public bool saveState;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.CONTROL, "CONTROL"),
        };
    }
}
