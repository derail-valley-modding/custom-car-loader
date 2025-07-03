using UnityEngine;

namespace CCL.Types.Proxies.Controls.VR
{
    [AddComponentMenu("CCL/Proxies/Controls/VR/Valve Hand Snapper Proxy")]
    public class ValveHandSnapperProxy : MonoBehaviour
    {
        [Tooltip("The Y axis points up")]
        public Transform axis = null!;

        public void OnDrawGizmosSelected()
        {
            if (axis != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(axis.position, axis.TransformPoint(Vector3.up * 0.1f));
            }
        }
    }
}
