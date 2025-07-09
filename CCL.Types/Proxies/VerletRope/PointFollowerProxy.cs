using UnityEngine;

namespace CCL.Types.Proxies.VerletRope
{
    [AddComponentMenu("CCL/Proxies/Verlet Rope/Point Follower Proxy")]
    public class PointFollowerProxy : MonoBehaviour
    {
        public RopeBehaviourProxy rope = null!;
        public int pointIndex;
        public bool reverseForward;
    }
}
