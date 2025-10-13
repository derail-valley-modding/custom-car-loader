using System;
using UnityEngine;

namespace CCL.Types
{
    public static class GizmoUtil
    {
        private const int GIZMO_SEGMENTS = 32;

        public static void DrawLocalPrism(Transform pivot, Vector3 center, Vector3 size)
        {
            var oldMatrix = Gizmos.matrix;

            Gizmos.matrix = pivot.localToWorldMatrix;
            Gizmos.DrawWireCube(center, size);

            Gizmos.matrix = oldMatrix;
        }

        public static void DrawLocalPrismSolid(Transform pivot, Vector3 center, Vector3 size)
        {
            var oldMatrix = Gizmos.matrix;

            Gizmos.matrix = pivot.localToWorldMatrix;
            Gizmos.DrawCube(center, size);

            Gizmos.matrix = oldMatrix;
        }

        public static void DrawLocalRay(Transform pivot, Vector3 start, Vector3 direction, float length)
        {
            var oldMatrix = Gizmos.matrix;

            Gizmos.matrix = pivot.localToWorldMatrix;
            Gizmos.DrawRay(start, direction.normalized * length);
            
            Gizmos.matrix = oldMatrix;
        }

        public static void DrawLocalCircle(Transform pivot, Vector3 axis, Color start, Color end, Color neutral, float radius, float angleOffset = 0)
        {
            // Always draw in local space to avoid axis issues, then transform the local space around the axis.
            using (new MatrixScope(pivot.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.back, axis))))
            {
                var cross = Vector3.up * radius;

                // To line up for easier editing.
                if (angleOffset != 0)
                {
                    cross = Quaternion.AngleAxis(angleOffset, Vector3.back) * cross;
                }

                Gizmos.color = neutral;
                Gizmos.DrawLine(Vector3.zero, cross);

                var prev = cross;
                var rot = Quaternion.AngleAxis(360.0f / GIZMO_SEGMENTS, Vector3.back);

                for (int i = 0; i < GIZMO_SEGMENTS; i++)
                {
                    var normal = (float)i / (GIZMO_SEGMENTS - 1);

                    cross = rot * cross;

                    Gizmos.color = Color.Lerp(start, end, normal);
                    Gizmos.DrawLine(prev, cross);

                    prev = cross;
                }
            }
        }

        public static void DrawLocalDirection(Transform pivot, Vector3 direction, Color c)
        {
            using (new MatrixScope(pivot.localToWorldMatrix))
            {
                Gizmos.color = c;
                Gizmos.DrawLine(Vector3.zero, direction);
            }
        }

        public static void DrawLocalDirection(Transform pivot, Vector3 direction, Vector3 offset, Color c)
        {
            using (new MatrixScope(pivot.localToWorldMatrix))
            {
                Gizmos.color = c;
                Gizmos.DrawLine(offset, offset + direction);
            }
        }

        public static void DrawLocalRotationArc(Transform pivot, float min, float max, Vector3 axis, Color start, Color end, Color neutral, float radius,
            float angleOffset = 0)
        {
            // Always draw in local space to avoid axis issues, then transform the local space around the axis.
            using (new MatrixScope(pivot.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.back, axis))))
            {
                var cross = Vector3.up * radius;

                // To line up for easier editing.
                if (angleOffset != 0)
                {
                    cross = Quaternion.AngleAxis(angleOffset, Vector3.back) * cross;
                }

                var mid = cross;

                // Rotate backwards to line up with minimum angle.
                cross = Quaternion.AngleAxis(min, Vector3.back) * cross;
                Gizmos.color = start;
                Gizmos.DrawLine(Vector3.zero, cross);

                var prev = cross;
                var rot = Quaternion.AngleAxis((max - min) / GIZMO_SEGMENTS, Vector3.back);

                for (int i = 0; i < GIZMO_SEGMENTS; i++)
                {
                    var normal = (float)i / GIZMO_SEGMENTS;

                    cross = rot * cross;

                    Gizmos.color = Color.Lerp(start, end, normal);
                    Gizmos.DrawLine(prev, cross);

                    prev = cross;
                }

                Gizmos.color = end;
                Gizmos.DrawLine(Vector3.zero, cross);

                // Only draw the neutral line if it doesn't overlap with min/max.
                if (min % 360 != 0 && max % 360 != 0)
                {
                    Gizmos.color = neutral;
                    Gizmos.DrawLine(Vector3.zero, mid);
                }
            }
        }

        public static void DrawLocalRotationArcDouble(Transform pivot, float min, float max, Vector3 axis, Color start, Color end, Color neutral, float radius,
            float angleOffset = 0)
        {
            // Always draw in local space to avoid axis issues, then transform the local space around the axis.
            using (new MatrixScope(pivot.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.back, axis))))
            {
                var cross = Vector3.up * radius;

                // To line up for easier editing.
                if (angleOffset != 0)
                {
                    cross = Quaternion.AngleAxis(angleOffset, Vector3.back) * cross;
                }

                var mid = cross;

                // Rotate backwards to line up with minimum angle.
                cross = Quaternion.AngleAxis(min, Vector3.back) * cross;
                Gizmos.color = start;
                Gizmos.DrawLine(-cross, cross);

                var prev = cross;
                var rot = Quaternion.AngleAxis((max - min) / GIZMO_SEGMENTS, Vector3.back);

                for (int i = 0; i < GIZMO_SEGMENTS; i++)
                {
                    var normal = (float)i / GIZMO_SEGMENTS;

                    cross = rot * cross;

                    Gizmos.color = Color.Lerp(start, end, normal);
                    Gizmos.DrawLine(prev, cross);
                    Gizmos.DrawLine(-prev, -cross);

                    prev = cross;
                }

                Gizmos.color = end;
                Gizmos.DrawLine(-cross, cross);

                // Only draw the neutral line if it doesn't overlap with min/max.
                if (min % 360 != 0 && max % 360 != 0)
                {
                    Gizmos.color = neutral;
                    Gizmos.DrawLine(-mid, mid);
                }
            }
        }

        public static void DrawLocalRotationNotches(Transform pivot, float min, float max, int notches, Vector3 axis, Color start, Color end, Color neutral,
            float radius, float angleOffset = 0)
        {
            // Always draw in local space to avoid axis issues, then transform the local space around the axis.
            using (new MatrixScope(pivot.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.back, axis))))
            {
                var cross = Vector3.up * radius;

                // To line up for easier editing.
                if (angleOffset != 0)
                {
                    cross = Quaternion.AngleAxis(angleOffset, Vector3.back) * cross;
                }

                var mid = cross;

                // Rotate backwards to line up with minimum angle.
                cross = Quaternion.AngleAxis(min, Vector3.back) * cross;
                Gizmos.color = start;
                Gizmos.DrawLine(Vector3.zero, cross);

                // Can't really do anything else.
                if (notches < 2) return;

                notches--;
                var rot = Quaternion.AngleAxis((max - min) / notches, Vector3.back);

                for (int i = 1; i <= notches; i++)
                {
                    var normal = (float)i / notches;

                    cross = rot * cross;

                    Gizmos.color = Color.Lerp(start, end, normal);
                    Gizmos.DrawLine(Vector3.zero, cross);
                }

                // Only draw the neutral line if it doesn't overlap with min/max.
                if (min % 360 != 0 && max % 360 != 0)
                {
                    Gizmos.color = neutral;
                    Gizmos.DrawLine(Vector3.zero, mid);
                }
            }
        }

        public static void DrawLocalRotationNotchesDouble(Transform pivot, float min, float max, int notches, Vector3 axis, Color start, Color end, Color neutral,
            float radius, float angleOffset = 0)
        {
            // Always draw in local space to avoid axis issues, then transform the local space around the axis.
            using (new MatrixScope(pivot.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.back, axis))))
            {
                var cross = Vector3.up * radius;

                // To line up for easier editing.
                if (angleOffset != 0)
                {
                    cross = Quaternion.AngleAxis(angleOffset, Vector3.back) * cross;
                }

                var mid = cross;

                // Rotate backwards to line up with minimum angle.
                cross = Quaternion.AngleAxis(min, Vector3.back) * cross;
                Gizmos.color = start;
                Gizmos.DrawLine(-cross, cross);

                // Can't really do anything else.
                if (notches < 2) return;

                notches--;
                var rot = Quaternion.AngleAxis((max - min) / notches, Vector3.back);

                for (int i = 1; i <= notches; i++)
                {
                    var normal = (float)i / notches;

                    cross = rot * cross;

                    Gizmos.color = Color.Lerp(start, end, normal);
                    Gizmos.DrawLine(-cross, cross);
                }

                // Only draw the neutral line if it doesn't overlap with min/max.
                if (min % 360 != 0 && max % 360 != 0)
                {
                    Gizmos.color = neutral;
                    Gizmos.DrawLine(Vector3.zero, mid);
                }
            }
        }

        public class MatrixScope : IDisposable
        {
            private readonly Matrix4x4 _matrix;

            public MatrixScope(Matrix4x4 matrix)
            {
                _matrix = Gizmos.matrix;
                Gizmos.matrix = matrix;
            }

            public void Dispose()
            {
                Gizmos.matrix = _matrix;
            }
        }
    }
}
