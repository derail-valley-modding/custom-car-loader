using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    [AddComponentMenu("CCL/Proxies/Controllers/Boiler Sim Controller Proxy")]
    public class BoilerSimControllerProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortValueType.GENERIC, true)]
        public string anglePortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(anglePortId), anglePortId, DVPortValueType.GENERIC)
        };
    }
}
