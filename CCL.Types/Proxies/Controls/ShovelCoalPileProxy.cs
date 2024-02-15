using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class ShovelCoalPileProxy : MonoBehaviour
    {
        [Tooltip("Whether this coal pile supplies infinite coal")]
        public bool isInfinite;
        [Tooltip("Applies if infinite is true")]
        public float coalChunkMass;
    }
}
