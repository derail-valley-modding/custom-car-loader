using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Generic Control Definition Proxy")]
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
