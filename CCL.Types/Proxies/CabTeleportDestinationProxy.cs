using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Cab Teleport Destination Proxy")]
    public class CabTeleportDestinationProxy : MonoBehaviour
    {
        private static readonly Color SELF = new Color(0.40f, 0.75f, 0.95f);
        private static readonly Color ROOM = new Color(0.95f, 0.75f, 0.40f);

        public TeleportHoverGlowProxy hoverGlow = null!;
        public Transform roomscaleTeleportPosition = null!;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = SELF;
            Gizmos.DrawWireSphere(transform.position, 0.1f);

            if (roomscaleTeleportPosition != null)
            {
                Gizmos.color = ROOM;
                Gizmos.DrawWireSphere(roomscaleTeleportPosition.position, 0.1f);
            }
        }
    }
}
