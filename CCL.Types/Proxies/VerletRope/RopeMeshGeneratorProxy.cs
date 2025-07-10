using UnityEngine;

namespace CCL.Types.Proxies.VerletRope
{
    [AddComponentMenu("CCL/Proxies/Verlet Rope/Rope Mesh Generator Proxy")]
    [RequireComponent(typeof(MeshFilter))]
    public class RopeMeshGeneratorProxy : MonoBehaviour
    {
        public float uvScale = 3f;
        public int interpolation = 2;
        public float thickness = 0.05f;
    }
}
