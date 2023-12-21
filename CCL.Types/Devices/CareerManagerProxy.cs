using UnityEngine;

namespace CCL.Types.Devices
{
    public class CareerManagerProxy : MonoBehaviour
    {
        // career manager dimensions
        private const float CM_HEIGHT = 1.765f;
        private const float CM_BOTTOM_HEIGHT = 0.946f;
        private const float CM_TOP_HEIGHT = 0.819f;
        private const float CM_WIDTH = 0.722f;
        private const float CM_DEPTH = 0.568f;
        private const float CM_TOP_DEPTH = 0.391f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Vector3 center = new Vector3(0, CM_BOTTOM_HEIGHT / 2, 0);
            GizmoUtil.DrawLocalPrism(transform, center, new Vector3(CM_DEPTH, CM_BOTTOM_HEIGHT, CM_WIDTH));

            center = new Vector3((CM_TOP_DEPTH / 2) - (CM_DEPTH / 2), CM_HEIGHT - (CM_TOP_HEIGHT / 2), 0);
            GizmoUtil.DrawLocalPrism(transform, center, new Vector3(CM_TOP_DEPTH, CM_TOP_HEIGHT, CM_WIDTH));
        }
    }
}
