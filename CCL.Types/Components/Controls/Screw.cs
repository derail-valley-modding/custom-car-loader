using CCL.Types.Proxies.Controls;
using UnityEngine;

namespace CCL.Types.Components.Controls
{
    [AddComponentMenu("CCL/Components/Controls/Screw")]
    public class Screw : ControlSpecProxy
    {
        private const int GIZMO_SEGMENTS_PER_REV = 40;
        private const int ANGLE = 360 / GIZMO_SEGMENTS_PER_REV;

        [Min(1)]
        public int Revolutions = 10;
        public bool InvertDirection = true;
        public float ScrollWheelHoverScroll = 100;
        public float Travel = 1;

        [Header("RigidBody")]
        public float Mass = 1;
        public float AngularDrag = 1;
        public bool ZeroCenterOfMass;

        [Header("Hinge Joint")]
        public float Spring = 5;
        public float Damper = 2;
        public float Bounciness;
        public float BounceMinVelocity;

        [Header("RotatorTrack")]
        public float RotatorMaxForceMagnitude = 0.01f;
        public float RotatorForceMultiplier = 0.1f;

        [Header("Haptics")]
        public bool UseHaptics = true;
        [EnableIf(nameof(UseHaptics))]
        public float NotchAngle = 1f;
        [EnableIf(nameof(UseHaptics))]
        public bool EnableWhenTouching;

        [Header("Audio")]
        public AudioClip? Drag;
        public AudioClip? LimitHit;
        public float HitTolerance = 0.1f;

        [Header("Editor Visualization")]
        public float GizmoScale = 0.5f;
        public float AngleOffset = 0;

        private void OnDrawGizmos()
        {
            using (new GizmoUtil.MatrixScope(transform.localToWorldMatrix))
            {
                var segments = Revolutions * GIZMO_SEGMENTS_PER_REV;
                var cross = new Vector3(0, GizmoScale, 0);
                var offset = new Vector3(0, 0, -Travel);
                var start = InvertDirection ? END_COLOR : START_COLOR;
                var end = InvertDirection ? START_COLOR : END_COLOR;

                // To line up for easier editing.
                if (AngleOffset != 0)
                {
                    cross = Quaternion.AngleAxis(AngleOffset, Vector3.forward) * cross;
                }

                Gizmos.color = start;
                Gizmos.DrawLine(Vector3.zero, cross);

                var prev = cross;
                var rot = Quaternion.AngleAxis(ANGLE, Vector3.forward);

                for (int i = 0; i < segments; i++)
                {
                    var normal = (float)i / segments;
                    var totalOffset = offset * ((i + 1.0f) / segments);

                    cross = rot * cross;

                    Gizmos.color = Color.Lerp(start, end, normal);
                    Gizmos.DrawLine(prev, cross + totalOffset);

                    prev = cross + totalOffset;
                }

                Gizmos.color = end;
                Gizmos.DrawLine(offset, cross + offset);
            }
        }
    }
}
