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

        public static void DrawLocalRay(Transform pivot, Vector3 start, Vector3 direction, float length)
        {
            var oldMatrix = Gizmos.matrix;

            Gizmos.matrix = pivot.localToWorldMatrix;
            Gizmos.DrawRay(start, direction.normalized * length);
            
            Gizmos.matrix = oldMatrix;
        }
    }
}
