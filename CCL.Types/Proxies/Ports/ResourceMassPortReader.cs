using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class ResourceMassPortReaderProxy : MonoBehaviour
    {
        [PortId(null, null, true)]
        public string resourceMassPortId;
    }
}
