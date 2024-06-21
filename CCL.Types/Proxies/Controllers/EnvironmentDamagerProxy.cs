using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class EnvironmentDamagerProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortType.EXTERNAL_IN, true)]
        public string damagerPortId;

        public BaseResourceType environmentDamageResource;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[] { new PortIdField(this, nameof(damagerPortId), damagerPortId, DVPortType.EXTERNAL_IN) };
    }
}
