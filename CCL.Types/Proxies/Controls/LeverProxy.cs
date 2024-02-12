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

        protected const float GIZMO_RADIUS = 0.1f;
        protected const int GIZMO_SEGMENTS = 40;
        protected static readonly Color START_COLOR = new Color(0.65f, 0, 0);
        protected static readonly Color END_COLOR = new Color(0, 0.65f, 0);

        private void OnDrawGizmos()
        {
            Color startColor = invertDirection ? END_COLOR : START_COLOR;
            Color endColor = invertDirection ? START_COLOR : END_COLOR;

            Vector3 massCenter = ModelUtil.GetModelCenterline(gameObject);
            Vector3 massZeroVector = transform.InverseTransformPoint(massCenter);
            Vector3 gizmoZeroVector = Vector3.ProjectOnPlane(massZeroVector, jointAxis);

            float massLength = massZeroVector.magnitude;
            Vector3 massPivot = Vector3.Project(massZeroVector, jointAxis);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.TransformPoint(massPivot));

            if (useSteppedJoint && (notches > 0))
            {
                float rayCount = notches - 1;

                // draw ray segments
                for (int i = 0; i <= rayCount; i++)
                {
                    Gizmos.color = Color.Lerp(startColor, endColor, i / rayCount);

                    float rotateAngle = Mathf.Lerp(jointLimitMin, jointLimitMax, i / rayCount);
                    Quaternion rotation = Quaternion.AngleAxis(rotateAngle, jointAxis);

                    // projected sweep
                    Vector3 rayVector = transform.TransformPoint((rotation * gizmoZeroVector) * GIZMO_RADIUS);
                    Gizmos.DrawLine(transform.position, rayVector);

                    // center of mass sweep
                    rayVector = transform.TransformPoint(rotation * massZeroVector);
                    Gizmos.DrawLine(transform.TransformPoint(massPivot), rayVector);
                }
            }
            else
            {
                // draw semi-circle
                Vector3 lastVector = transform.position;
                Vector3 lastMassVector = transform.TransformPoint(massPivot);

                for (int i = 0; i <= GIZMO_SEGMENTS; i++)
                {
                    Gizmos.color = Color.Lerp(startColor, endColor, (float)i / GIZMO_SEGMENTS);

                    float rotateAngle = Mathf.Lerp(jointLimitMin, jointLimitMax, (float)i / GIZMO_SEGMENTS);
                    Quaternion rotation = Quaternion.AngleAxis(rotateAngle, jointAxis);

                    // projected sweep
                    Vector3 nextVector = transform.TransformPoint((rotation * gizmoZeroVector) * GIZMO_RADIUS);
                    Vector3 nextMassVector = transform.TransformPoint(rotation * massZeroVector);

                    if (i == 0 || i == GIZMO_SEGMENTS)
                    {
                        Gizmos.DrawLine(transform.position, nextVector);
                        Gizmos.DrawLine(transform.TransformPoint(massPivot), nextMassVector);
                    }
                    if (i != 0)
                    {
                        Gizmos.DrawLine(lastVector, nextVector);
                        Gizmos.DrawLine(lastMassVector, nextMassVector);
                    }

                    lastVector = nextVector;
                    lastMassVector = nextMassVector;
                }
            }
        }
    }
}
