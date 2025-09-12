using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Wheel Proxy")]
    public class WheelProxy : ControlSpecProxy
    {
        [Header("RigidBody")]
        public float mass = 1f;
        public float angularDrag = 1f;

        [Header("Hinge Joint")]
        public Vector3 jointAxis = Vector3.up;
        public bool useSpring;
        public float jointSpring;
        public float springDamper;

        public bool useLimits = true;
        public float jointLimitMin;
        public float jointLimitMax;

        public float bounciness;
        public float bounceMinVelocity;

        [Tooltip("A value between joint min and max limit")]
        public float jointStartingPos;

        public bool invertDirection = true;
        public float scrollWheelHoverScroll = 1f;

        [Header("RotatorTrack")]
        public float rotatorMaxForceMagnitude = 0.1f;

        [Header("Audio")]
        public AudioClip drag = null!;
        public AudioClip limitHit = null!;

        public float hitTolerance = 0.1f;

        [Header("Haptics")]
        public bool useHaptics = true;
        public float notchAngle = 1f;
        public bool enableWhenTouching;

        [Header("Editor visualization")]
        public float gizmoScale = 0.5f;

        protected const int GIZMO_SEGMENTS = 40;
        protected static readonly Color START_COLOR = new Color(0.65f, 0, 0);
        protected static readonly Color END_COLOR = new Color(0, 0.65f, 0);

        public override void OnValidate()
        {
            jointLimitMin = Mathf.Clamp(jointLimitMin, -177f, 177f);
            jointLimitMax = Mathf.Clamp(jointLimitMax, -177f, 177f);

            base.OnValidate();
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawLine(Vector3.zero, jointAxis * gizmoScale);

            var cross = Vector3.Cross(jointAxis, Quaternion.Euler(90, 0, 0) * jointAxis).normalized * gizmoScale;

            if (useLimits)
            {
                cross = Quaternion.AngleAxis(jointLimitMin, jointAxis) * cross;
                Gizmos.color = START_COLOR;
                Gizmos.DrawLine(Vector3.zero, cross);
            }

            var prev = cross;
            var rot = Quaternion.AngleAxis((useLimits ? jointLimitMax - jointLimitMin : 360.0f) / GIZMO_SEGMENTS, jointAxis);

            for (int i = 0; i < GIZMO_SEGMENTS; i++)
            {
                var normal = (float)i / GIZMO_SEGMENTS;

                cross = rot * cross;

                Gizmos.color = Color.Lerp(START_COLOR, END_COLOR, normal);
                Gizmos.DrawLine(prev, cross);

                prev = cross;
            }

            if (useLimits)
            {
                Gizmos.color = END_COLOR;
                Gizmos.DrawLine(Vector3.zero, cross);
            }

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
