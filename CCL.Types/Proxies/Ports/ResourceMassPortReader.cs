using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    [AddComponentMenu("CCL/Proxies/Ports/Resource Mass Port Reader Proxy")]
    public class ResourceMassPortReaderProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(null, null, true)]
        public string resourceMassPortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(resourceMassPortId), resourceMassPortId),
        };
    }
}
