using UnityEngine;

namespace CCL.Types.Proxies
{
    public class InvalidTeleportLocationReactionProxy : MonoBehaviour
    {
        public ZoneBlockerProxy blocker;
        public float waitBeforeSpawn = 1f;
        public bool drawAttentionPointLine = true;
    }
}
