using System.Collections;
using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public class RotarySetup : ControlSetupBase
    {
        public override string TargetTypeName => "DV.CabControls.Spec.Rotary";
        public override bool DestroyAfterCreation => true;
        public override CabControlType ControlType => CabControlType.Rotary;

        [Header("Knob Settings")]
        [ProxyField]
        public bool useSteppedJoint = true;
        [ProxyField]
        public int notches = 3;
        [ProxyField]
        public bool invertDirection = false;
        [ProxyField("scrollWheelHoverScroll")]
        public float scrollWheelDelta = 1f;
        [ProxyField]
        public bool scrollWheelUseSpringRotation = false;

        [Header("Hinge Joint")]
        [ProxyField]
        public Vector3 jointAxis = Vector3.up;
        [ProxyField]
        public bool useSpring = true;
        [ProxyField]
        public float jointSpring = 0.5f;
        [ProxyField]
        public float jointDamper;
        [ProxyField]
        public bool useLimits = true;
        [ProxyField]
        public float jointLimitMin;
        [ProxyField]
        public float jointLimitMax;
        [ProxyField]
        public float bounciness;
        [ProxyField]
        public float bounceMinVelocity;

        [Tooltip("A value between joint min and max limit")]
        [ProxyField]
        public float jointStartingPos;

        [Header("Physics")]
        [ProxyField]
        public float rigidbodyMass = 1f;
        [ProxyField]
        public float rigidbodyAngularDrag = 0.03f;

        [ProxyComponent("nonVrStaticInteractionArea", "StaticInteractionArea")]
        public GameObject StaticInteractionArea = null;

        protected const int GIZMO_SLICE_SIZE = 10;
        protected const float GIZMO_RADIUS = 0.02f;
        protected static readonly Color START_COLOR = new Color(0.65f, 0, 0);
        protected static readonly Color END_COLOR = new Color(0, 0.65f, 0);

        private void OnDrawGizmos()
        {
            Vector3 lastVector = transform.position;
            Vector3 lastOpp = transform.position;

            foreach(float end in new[] { jointLimitMin, jointLimitMax })
            {
                if(jointStartingPos != end)
                {
                    float totalSweep = Mathf.Abs(end - jointStartingPos);
                    int gizmoSegments = Mathf.FloorToInt(totalSweep / GIZMO_SLICE_SIZE);

                    for(int i = 0; i <= gizmoSegments; i++)
                    {
                        Gizmos.color = Color.Lerp(START_COLOR, END_COLOR, (float)i / gizmoSegments);
                        Vector3 radialVector = (Quaternion.AngleAxis(
                            Mathf.Lerp(jointStartingPos, end, (float)i / gizmoSegments), jointAxis)
                            * Vector3.forward).normalized;

                        Vector3 nextVector = transform.TransformPoint(radialVector * GIZMO_RADIUS);
                        Vector3 oppositeVector = transform.TransformPoint(radialVector * -GIZMO_RADIUS);

                        if(i != 0)
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