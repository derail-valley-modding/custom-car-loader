using CCL.Types.Proxies.Controls.VR;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Drawing
{
    internal class SnapperDrawer
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        private static void DrawGizmoParticleSystem(LineHandSnapperProxy snapper, GizmoType gizmoType)
        {
            if (snapper.lineStart == null) return;

            var start = snapper.lineStart.position;
            var end = snapper.lineStart.TransformPoint(Vector3.up * snapper.lineLength);
            var dir = end - start;

            var tAngle = snapper.maxAngle - snapper.minAngle;

            if (tAngle >= 360)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(start, end);
                Gizmos.DrawSphere(start, 0.01f);
                Gizmos.DrawSphere(end, 0.01f);
            }
            else
            {
                var from = snapper.lineStart.TransformDirection(Quaternion.AngleAxis(snapper.minAngle, Vector3.up) * Vector3.right);

                Handles.color = Color.cyan;

                Handles.DrawLine(start, end);
                Handles.DrawSolidArc(start, dir, from, tAngle, 0.01f);
                Handles.DrawSolidArc(end, dir, from, tAngle, 0.01f);
                Handles.DrawWireArc(start, dir, from, 360, 0.01f);
                Handles.DrawWireArc(end, dir, from, 360, 0.01f);
            }
        }
    }
}
