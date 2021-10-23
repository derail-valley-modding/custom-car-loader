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

        protected const float GIZMO_RADIUS = 0.05f;
        protected const int GIZMO_SEGMENTS = 10;
        protected static readonly Color START_COLOR = new Color(0.65f, 0, 0);
        protected static readonly Color END_COLOR = new Color(0, 0.65f, 0);

        private void OnDrawGizmos()
        {
            Vector3 lastVector = transform.position;

            // draw ray segments
            for(int i = 0; i <= GIZMO_SEGMENTS; i++)
            {
                Gizmos.color = Color.Lerp(START_COLOR, END_COLOR, (float)i / GIZMO_SEGMENTS);

                float rotateAngle = Mathf.Lerp(jointLimitMin, jointLimitMax, (float)i / GIZMO_SEGMENTS);
                Quaternion rotation = Quaternion.AngleAxis(rotateAngle, jointAxis);

                // projected sweep
                Vector3 nextVector = transform.TransformPoint((rotation * Vector3.forward) * GIZMO_RADIUS);

                if(i == 0 || i == GIZMO_SEGMENTS)
                {
                    Gizmos.DrawLine(transform.position, nextVector);
                }
                if(i != 0)
                {
                    Gizmos.DrawLine(lastVector, nextVector);
                }

                lastVector = nextVector;
            }
        }
    }
}