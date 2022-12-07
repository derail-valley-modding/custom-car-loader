using System.Collections;
using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public class ToggleSetup : ControlSetupBase
    {
        public override string TargetTypeName => "DV.CabControls.Spec.ToggleSwitch";
        public override bool DestroyAfterCreation => true;
        public override CabControlType ControlType => CabControlType.Toggle;

        [ProxyField]
        public Vector3 jointAxis = Vector3.forward;
        [ProxyField]
        public float jointLimitMin;
        [ProxyField]
        public float jointLimitMax;
        [ProxyField]
        public float autoOffTimer;

        [ProxyComponent("nonVrStaticInteractionArea", "StaticInteractionArea")]
        public GameObject StaticInteractionArea = null;

        protected const float GIZMO_RADIUS = 0.05f;
        protected const int GIZMO_SEGMENTS = 10;
        protected static readonly Color START_COLOR = new Color(0.65f, 0, 0);
        protected static readonly Color END_COLOR = new Color(0, 0.65f, 0);

        private void OnDrawGizmos()
        {
            Vector3 massCenter = ModelUtil.GetModelCenterline(gameObject);
            Vector3 massZeroVector = transform.InverseTransformPoint(massCenter);
            Vector3 gizmoZeroVector = Vector3.ProjectOnPlane(massZeroVector, jointAxis);
            Vector3 massPivot = Vector3.Project(massZeroVector, jointAxis);

            Vector3 lastVector = transform.position;
            Vector3 lastMassVector = transform.TransformPoint(massPivot);

            // draw ray segments
            for (int i = 0; i <= GIZMO_SEGMENTS; i++)
            {
                Gizmos.color = Color.Lerp(START_COLOR, END_COLOR, (float)i / GIZMO_SEGMENTS);

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
                if(i != 0)
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