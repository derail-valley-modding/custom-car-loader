using UnityEngine;

namespace CCL.Types.Proxies.VerletRope
{
    [AddComponentMenu("CCL/Proxies/Verlet Rope/Rope Pin")]
    public class RopePin : MonoBehaviour
    {
        public int pointIndex;
        public Vector3 pinLocalPos;
        public bool active;
        public Transform pinnedToTransform = null!;
        public float addedBendingCorrection;

        private void OnReset()
        {
            pinnedToTransform = transform;
        }
    }
}
