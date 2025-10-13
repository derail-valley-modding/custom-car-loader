using UnityEngine;

namespace CCL.Types.Devices
{
    public class JobTrashBinProxy : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.9f, 0.6f, 0.2f);

            // Body.
            Vector3 center = new Vector3(0.0f, 0.33f, 0.0f);
            GizmoUtil.DrawLocalPrism(transform, center, new Vector3(0.44f, 0.66f, 0.35f));

            // Lid.
            center = new Vector3(0.0f, 0.841f, -0.255f);
            GizmoUtil.DrawLocalPrism(transform, center, new Vector3(0.5f, 0.4f, 0.05f));
        }
    }
}
