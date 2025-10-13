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
            Gizmos.color = Color.green;
            GizmoUtil.DrawLocalPrismSolid(transform, center, size);
        }
    }
}
