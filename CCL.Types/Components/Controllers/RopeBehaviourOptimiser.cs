using CCL.Types.Proxies.VerletRope;
using UnityEngine;

namespace CCL.Types.Components.Controllers
{
    [AddComponentMenu("CCL/Components/Controllers/Rope Behaviour Optimiser")]
    public class RopeBehaviourOptimiser : MonoBehaviour
    {
        public RopeBehaviourProxy[] Ropes = new RopeBehaviourProxy[0];
        public int FullIterations = 100;
        public int SlowIterations = 5;
        public float DistanceSlow = 150.0f;
        public float DistanceDisable = 500.0f;
    }
}
