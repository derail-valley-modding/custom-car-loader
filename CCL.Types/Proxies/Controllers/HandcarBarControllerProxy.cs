using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    [AddComponentMenu("CCL/Proxies/Controllers/Handcar Bar Controller Proxy")]
    public class HandcarBarControllerProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, false)]
        public string handleEngagedPortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(handleEngagedPortId), handleEngagedPortId, DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL)
        };
    }
}
