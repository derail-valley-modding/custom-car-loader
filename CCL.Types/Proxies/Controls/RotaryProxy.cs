using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Rotary Proxy")]
    public class RotaryProxy : ControlSpecProxy
    {
        public float rigidbodyMass = 1f;
        public float rigidbodyAngularDrag = 0.03f;
        public float blockAngularDrag;

        [Header("Rotary (knob)")]
        public bool invertDirection;
        public float scrollWheelHoverScroll = 1f;
        public bool scrollWheelUseSpringRotation;

        [Header("Notches")]
        public bool useSteppedJoint = true;
        public int notches = 20;
        public bool useInnerLimitSpring;
        public int innerLimitMinNotch;
        public int innerLimitMaxNotch;

        [Header("Hinge Joint")]
        public Vector3 jointAxis = Vector3.up;
        public bool useSpring = true;
        public float jointSpring = 0.5f;
        public float jointDamper;
        public bool useLimits = true;
        public float jointLimitMin;
        public float jointLimitMax;
        public float bounciness;
        public float bounceMinVelocity;

        [Tooltip("A value between joint min and max limit")]
        public float jointStartingPos;

        [Header("Audio")]
        public AudioClip notch = null!;
        public AudioClip drag = null!;
        public AudioClip limitHit = null!;

        [Header("Editor visualization")]
        public float gizmoRadius = 0.02f;
        public float angleOffset = 0;

        private void OnDrawGizmos()
        {
            Color startColor = invertDirection ? END_COLOR : START_COLOR;
            Color endColor = invertDirection ? START_COLOR : END_COLOR;

            if (useSteppedJoint)
            {
                GizmoUtil.DrawLocalRotationNotchesDouble(transform, jointLimitMin, jointLimitMax, notches, jointAxis,
                    startColor, endColor, MID_COLOR, gizmoRadius, angleOffset);
            }

            GizmoUtil.DrawLocalRotationArcDouble(transform, jointLimitMin, jointLimitMax, jointAxis,
                startColor, endColor, MID_COLOR, gizmoRadius, angleOffset);
        }
    }
}
