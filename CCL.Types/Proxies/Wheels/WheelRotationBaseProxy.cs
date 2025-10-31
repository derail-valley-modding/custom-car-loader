using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    public abstract class WheelRotationBaseProxy : MonoBehaviour
    {
        private const int GIZMO_SEGMENTS = 18;
        private static readonly Color START_POWERED = new Color(0.85f, 0.95f, 0.15f);
        private static readonly Color START_UNPOWERED = new Color(0.95f, 0.15f, 0.85f);
        private static readonly Color END = new Color(0.15f, 0.95f, 0.85f);
        private static readonly Vector3 ROT_AXIS = Vector3.left;

        public float wheelRadius = 0.7f;
        public bool affectedByWheelSlide = true;

        public static void DrawWheelGizmo(Transform t, Vector3 axis, float radius, bool powered)
        {
            var start = powered ? START_POWERED : START_UNPOWERED;

            using (new GizmoUtil.MatrixScope(t.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.FromToRotation(ROT_AXIS, axis))))
            {
                Gizmos.color = start;
                Gizmos.DrawLine(ROT_AXIS, -ROT_AXIS);

                var cross = Quaternion.AngleAxis(-45, ROT_AXIS) * (Vector3.up * radius);

                var prev = cross;
                var rot = Quaternion.AngleAxis(90.0f / GIZMO_SEGMENTS, ROT_AXIS);

                Gizmos.DrawWireSphere(Vector3.zero, radius * 0.1f);

                for (int i = 0; i < GIZMO_SEGMENTS; i++)
                {
                    var normal = (float)i / GIZMO_SEGMENTS;

                    cross = rot * cross;

                    Gizmos.color = Color.Lerp(start, END, normal);
                    Gizmos.DrawLine(prev, cross);
                    Gizmos.DrawLine(-prev, -cross);

                    prev = cross;
                }

                // Draw a small arrow at the end of the arc.
                var size = radius * 0.3f;
                var up = Vector3.up * size;
                var fwd = Vector3.forward * size;

                Gizmos.color = END;
                Gizmos.DrawLine(prev, prev + up);
                Gizmos.DrawLine(prev, prev + fwd);
                Gizmos.DrawLine(-prev, -prev - up);
                Gizmos.DrawLine(-prev, -prev - fwd);
            }
        }
    }
}
