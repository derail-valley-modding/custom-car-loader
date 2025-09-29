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
        public float angleOffset = 0;

        protected const int GIZMO_SEGMENTS = 40;

        public override void OnValidate()
        {
            jointLimitMin = Mathf.Clamp(jointLimitMin, -177f, 177f);
            jointLimitMax = Mathf.Clamp(jointLimitMax, -177f, 177f);

            base.OnValidate();
        }

        private void OnDrawGizmos()
        {
            if (useLimits)
            {
                GizmoUtil.DrawLocalRotationArc(transform, jointLimitMin, jointLimitMax, jointAxis,
                    START_COLOR, END_COLOR, MID_COLOR, gizmoScale, angleOffset);
            }
            else
            {
                GizmoUtil.DrawLocalCircle(transform, jointAxis, START_COLOR, END_COLOR, MID_COLOR, gizmoScale, angleOffset);
            }
        }
    }
}
