using CCL.Types.Proxies.Ports;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public abstract class PoweredControlHandlerBase : MonoBehaviour
    {
        [PortId(DVPortValueType.CONTROL, false)]
        public string controlId;

        [FuseId]
        public string powerFuseId;
    }
}
