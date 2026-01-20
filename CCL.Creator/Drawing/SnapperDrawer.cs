using CCL.Types;
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

            var end = Vector3.up * snapper.lineLength;
            var from = Quaternion.Euler(0, -snapper.minAngle, 0) * Vector3.right;
            var tAngle = snapper.maxAngle - snapper.minAngle;

            Handles.matrix = snapper.lineStart.localToWorldMatrix;
            Handles.color = Color.cyan;

            Handles.DrawLine(Vector3.zero, end);
            Handles.DrawSolidArc(Vector3.zero, Vector3.up, from, -tAngle, 0.01f);
            Handles.DrawSolidArc(end, Vector3.up, from, -tAngle, 0.01f);
            Handles.DrawWireArc(Vector3.zero, Vector3.up, from, 360, 0.01f);
            Handles.DrawWireArc(end, Vector3.up, from, 360, 0.01f);

            Handles.matrix = Matrix4x4.identity;
        }
    }
}
