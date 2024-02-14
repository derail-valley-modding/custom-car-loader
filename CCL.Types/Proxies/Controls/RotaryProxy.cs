using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
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
        public AudioClip notch;
        public AudioClip drag;
        public AudioClip limitHit;


        protected const int GIZMO_SLICE_SIZE = 10;
        protected const float GIZMO_RADIUS = 0.02f;
        protected static readonly Color START_COLOR = new Color(0.65f, 0, 0);
        protected static readonly Color END_COLOR = new Color(0, 0.65f, 0);

        private void OnDrawGizmos()
        {
            Vector3 lastVector = transform.position;
            Vector3 lastOpp = transform.position;

            foreach (float end in new[] { jointLimitMin, jointLimitMax })
            {
                if (jointStartingPos != end)
                {
                    float totalSweep = Mathf.Abs(end - jointStartingPos);
                    int gizmoSegments = Mathf.FloorToInt(totalSweep / GIZMO_SLICE_SIZE);

                    for (int i = 0; i <= gizmoSegments; i++)
                    {
                        Gizmos.color = Color.Lerp(START_COLOR, END_COLOR, (float)i / gizmoSegments);
                        Vector3 radialVector = (Quaternion.AngleAxis(
                            Mathf.Lerp(jointStartingPos, end, (float)i / gizmoSegments), jointAxis)
                            * Vector3.forward).normalized;

                        Vector3 nextVector = transform.TransformPoint(radialVector * GIZMO_RADIUS);
                        Vector3 oppositeVector = transform.TransformPoint(radialVector * -GIZMO_RADIUS);

                        if (i != 0)
                        {
                            Gizmos.DrawLine(lastVector, nextVector);
                            Gizmos.DrawLine(lastOpp, oppositeVector);
                        }

                        Gizmos.DrawLine(transform.position, nextVector);
                        Gizmos.DrawLine(transform.position, oppositeVector);

                        lastVector = nextVector;
                        lastOpp = oppositeVector;
                    }
                }
            }
        }
    }
}
