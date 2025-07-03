using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Loco Zone Blocker Proxy")]
    public class LocoZoneBlockerProxy : ZoneBlockerProxy
    {
        public CabTeleportDestinationProxy cab = null!;

        private void OnReset()
        {
            if (blockerObjectsParent == null)
            {
                blockerObjectsParent = gameObject;
            }
        }
    }
}
