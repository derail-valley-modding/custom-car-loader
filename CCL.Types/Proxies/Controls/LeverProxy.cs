using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
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
        public Transform interactionPoint;

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
        public AudioClip notch;
        public AudioClip drag;
        public AudioClip limitHit;
        public bool limitVibration;

        [Header("Editor visualization")]
        public float gizmoRadius = 0.5f;

        protected const int GIZMO_SEGMENTS = 40;
        protected static readonly Color START_COLOR = new Color(0.65f, 0, 0);
        protected static readonly Color END_COLOR = new Color(0, 0.65f, 0);

        private void OnDrawGizmos()
        {
            Color startColor = invertDirection ? END_COLOR : START_COLOR;
            Color endColor = invertDirection ? START_COLOR : END_COLOR;

            Vector3 drawAxis = Mathf.Abs(Vector3.Dot(jointAxis, Vector3.up)) > 0.9f ? Vector3.forward : Vector3.up;

            if (useSteppedJoint && notches > 0)
            {
                notches--;

                for (int i = 0; i <= notches; i++)
                {
                    Gizmos.color = Color.Lerp(startColor, endColor, (float)i / notches);
                    Vector3 position = transform.TransformPoint(
                        Quaternion.AngleAxis(Mathf.Lerp(jointLimitMin, jointLimitMax, (float)i / notches), jointAxis) * drawAxis * gizmoRadius);
                    
                    Gizmos.DrawLine(transform.position, position);
                }

                notches++;
            }
            else
            {
                Vector3 start = Vector3.zero;

                for (int i = 0; i <= GIZMO_SEGMENTS; i++)
                {
                    Gizmos.color = Color.Lerp(startColor, endColor, (float)i / GIZMO_SEGMENTS);
                    Vector3 position = transform.TransformPoint(
                        Quaternion.AngleAxis(Mathf.Lerp(jointLimitMin, jointLimitMax, (float)i / GIZMO_SEGMENTS), jointAxis) * drawAxis * gizmoRadius);

                    if (i == 0 || i == GIZMO_SEGMENTS)
                    {
                        Gizmos.DrawLine(transform.position, position);
                    }

                    if (i != 0)
                    {
                        Gizmos.DrawLine(start, position);
                    }

                    start = position;
                }
            }
        }
    }
}
