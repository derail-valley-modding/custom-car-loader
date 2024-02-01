using CCL.Types.Proxies;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    internal class DistanceDrawer
    {
        public static void DrawFloorCircle(Vector3 position, float radius)
        {
            Handles.DrawWireArc(position, Vector3.up, Vector3.forward, 360.0f, radius);
        }

        [DrawGizmo(GizmoType.Selected)]
        private static void DrawGizmoDistanceOptimiser(PlayerDistanceGameObjectsDisablerProxy proxy, GizmoType gizmoType)
        {
            Gizmos.color = new Color(0.8f, 0.9f, 1.0f);
            DrawFloorCircle(proxy.transform.position, proxy.disableDistance);
        }

        [DrawGizmo(GizmoType.Selected)]
        private static void DrawGizmoDistanceOptimiser(PlayerDistanceMultipleGameObjectsOptimizerProxy proxy, GizmoType gizmoType)
        {
            Gizmos.color = new Color(0.8f, 0.9f, 1.0f);
            DrawFloorCircle(proxy.transform.position, proxy.disableDistance);
        }
    }
}
