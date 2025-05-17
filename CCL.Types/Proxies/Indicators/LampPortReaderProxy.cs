using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CCL.Types.Proxies.Indicators
{
    public class LampPortReaderProxy : MonoBehaviour, IHasPortIdFields
    {
        [FormerlySerializedAs("portId")]
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.STATE, false)]
        public string lampStatePortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(lampStatePortId), lampStatePortId, DVPortType.READONLY_OUT, DVPortValueType.STATE),
        };
    }
}
