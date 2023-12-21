using UnityEngine;

namespace CCL.Types
{
    public static class GizmoUtil
    {
        public static void DrawLocalPrism(Transform pivot, Vector3 center, Vector3 size)
        {
            var oldMatrix = Gizmos.matrix;

            Gizmos.matrix = pivot.localToWorldMatrix;
            Gizmos.DrawWireCube(center, size);

            Gizmos.matrix = oldMatrix;
        }
    }
}
