using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class ResourceMassPortReaderProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(null, null, true)]
        public string resourceMassPortId;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(resourceMassPortId), resourceMassPortId),
        };
    }
}
