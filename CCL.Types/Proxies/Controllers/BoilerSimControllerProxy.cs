using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class BoilerSimControllerProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortValueType.GENERIC, true)]
        public string anglePortId;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[] { new PortIdField(this, nameof(anglePortId), anglePortId, DVPortValueType.GENERIC) };
    }
}
