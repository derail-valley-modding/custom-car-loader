using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Lever Proxy")]
    public class LeverProxy : ControlSpecProxy
    {
        [Header("Rigidbody")]
        public float rigidbodyMass = 30f;
        public float rigidbodyDrag = 15f;
        public float rigidbodyAngularDrag;
        public float blockDrag;
        public float blockAngularDrag;

        [Header("Lever")]
        public bool invertDirection;

        [Tooltip("Optional")]
        public Transform interactionPoint = null!;

        public float maxForceAppliedMagnitude = float.PositiveInfinity;
        public float pullingForceMultiplier = 1f;
        public float scrollWheelHoverScroll = 0.025f;
        public float scrollWheelSpring;

        [Header("Notches")]
        public bool useSteppedJoint = true;
        public bool steppedValueUpdate = true;
        public int notches = 20;
        public bool useInnerLimitSpring;

        public int innerLimitMinNotch;
        public int innerLimitMaxNotch;

        [Header("Hinge Joint")]
        public Vector3 jointAxis = Vector3.up;

        public bool useSpring = true;
        public float jointSpring = 100f;
        public float jointDamper;
        public bool useLimits = true;
        public float jointLimitMin;
        public float jointLimitMax;

        [Header("Audio")]
        public AudioClip notch = null!;
        public AudioClip drag = null!;
        public AudioClip limitHit = null!;
        public bool limitVibration;

        [Header("Editor visualization")]
        public float gizmoRadius = 0.5f;
        public float angleOffset = 0;

        private void OnDrawGizmos()
        {
            Color startColor = invertDirection ? END_COLOR : START_COLOR;
            Color endColor = invertDirection ? START_COLOR : END_COLOR;

            if (useSteppedJoint)
            {
                GizmoUtil.DrawLocalRotationNotches(transform, jointLimitMin, jointLimitMax, notches, jointAxis,
                    startColor, endColor, MID_COLOR, gizmoRadius, angleOffset);
                GizmoUtil.DrawLocalRotationArc(transform, jointLimitMin, jointLimitMax, jointAxis,
                    startColor, endColor, MID_COLOR, gizmoRadius * 0.25f, angleOffset);
            }
            else
            {
                GizmoUtil.DrawLocalRotationArc(transform, jointLimitMin, jointLimitMax, jointAxis,
                    startColor, endColor, MID_COLOR, gizmoRadius, angleOffset);
            }
        }
    }
}
