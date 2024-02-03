using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public class IndicatorGaugeProxy : IndicatorProxy
    {
        public Transform needle;
        public float minAngle = -180f;
        public float maxAngle = 180f;
        public bool unclamped;
        public Vector3 rotationAxis = Vector3.forward;
        public float gizmoRadius = 0.1f;


        protected const float GIZMO_RADIUS = 0.1f;
        protected const int GIZMO_SEGMENTS = 20;
        protected static readonly Color END_COLOR = new Color(0, 0, 0.65f);
        protected static readonly Color START_COLOR = new Color(0.65f, 0, 0);

        private void OnDrawGizmos()
        {
            if (!needle)
            {
                return;
            }

            Vector3 massCenter = ModelUtil.GetModelCenterline(gameObject);
            Vector3 massZeroVector = transform.InverseTransformPoint(massCenter);
            Vector3 massPivot = Vector3.Project(massZeroVector, rotationAxis);

            Vector3 lastVector = Vector3.zero;
            for (int i = 0; i <= GIZMO_SEGMENTS; i++)
            {
                Color segmentColor = Color.Lerp(START_COLOR, END_COLOR, (float)i / GIZMO_SEGMENTS);

                Quaternion rotation = Quaternion.AngleAxis(Mathf.Lerp(minAngle, maxAngle, (float)i / GIZMO_SEGMENTS), rotationAxis);
                Vector3 nextVector = transform.TransformPoint(rotation * massZeroVector);
                
                Gizmos.color = segmentColor;
                if (i == 0 || i == GIZMO_SEGMENTS)
                {
                    Gizmos.DrawLine(transform.TransformPoint(massPivot), nextVector);
                }

                if (i != 0)
                {
                    Gizmos.DrawLine(lastVector, nextVector);
                }

                lastVector = nextVector;
            }
        }
    }

    public class IndicatorGaugeLaggingProxy : IndicatorGaugeProxy
    {
        [Range(0f, 1f)]
        public float needleAgility = 0.5f;
    }
}
