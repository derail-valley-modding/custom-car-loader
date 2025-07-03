using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Cab Teleport Destination Proxy")]
    public class CabTeleportDestinationProxy : MonoBehaviour
    {
        public TeleportHoverGlowProxy hoverGlow = null!;
        public Transform roomscaleTeleportPosition = null!;
    }
}
