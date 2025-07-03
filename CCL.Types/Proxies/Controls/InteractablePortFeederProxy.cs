using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Interactable Port Feeder Proxy")]
    public class InteractablePortFeederProxy : MonoBehaviour, IHasPortIdFields
    {
        // This is an EXT_IN port which controls something, and is not required to be on the same prefab (but is on the same traincar
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, false)]
        public string portId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(portId), portId, DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL),
        };
    }
}
