using UnityEngine;

namespace CCL.Types.Proxies.Cargo
{
    [AddComponentMenu("CCL/Proxies/Cargo/Cargo Bounds Proxy")]
    public class CargoBoundsProxy : MonoBehaviour
    {
        public Vector3 center;
        public Vector3 size;

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(center, size);
            }
        }
    }
}
