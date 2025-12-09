using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    [AddComponentMenu("CCL/Proxies/Controllers/Environment Damager Proxy")]
    public class EnvironmentDamagerProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortType.EXTERNAL_IN, true)]
        public string damagerPortId = string.Empty;
        public BaseResourceType environmentDamageResource;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(damagerPortId), damagerPortId, DVPortType.EXTERNAL_IN)
        };
    }
}
