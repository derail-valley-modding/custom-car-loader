using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCL.Types.Proxies.Controls
{
    public class ExternalControlDefinitionProxy : SimComponentDefinitionProxy
    {
        public float defaultValue;
        public bool saveState;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "EXT_IN")
        };
    }
}
