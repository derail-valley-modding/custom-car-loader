using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class EngineOnReaderProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.STATE, false, local = true)]
        public string portId;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[] { new PortIdField(this, nameof(portId), portId, DVPortType.READONLY_OUT, DVPortValueType.STATE) };
    }
}
