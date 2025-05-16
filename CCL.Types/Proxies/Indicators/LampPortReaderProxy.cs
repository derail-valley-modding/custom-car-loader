using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public class LampPortReaderProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.STATE, false)]
        public string portId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(portId), portId),
        };
    }
}
