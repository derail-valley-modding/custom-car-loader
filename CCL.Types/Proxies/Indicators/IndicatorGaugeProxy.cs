using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    [AddComponentMenu("CCL/Proxies/Indicators/Indicator Gauge Proxy")]
    public class IndicatorGaugeProxy : IndicatorProxy
    {
        public Transform needle = null!;
        public float minAngle = -180f;
        public float maxAngle = 180f;
        public bool unclamped;
        public Vector3 rotationAxis = Vector3.back;

        [Header("Editor visualization")]
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

            Vector3 start = Vector3.zero;

            for (int i = 0; i <= GIZMO_SEGMENTS; i++)
            {
                Gizmos.color = Color.Lerp(START_COLOR, END_COLOR, (float)i / GIZMO_SEGMENTS);
                Vector3 position = transform.TransformPoint(
                    Quaternion.AngleAxis(Mathf.Lerp(minAngle, maxAngle, (float)i / GIZMO_SEGMENTS), rotationAxis) * Vector3.up * gizmoRadius);

                if (i == 0 || i == GIZMO_SEGMENTS)
                {
                    Gizmos.DrawLine(needle.position, position);
                }

                if (i != 0)
                {
                    Gizmos.DrawLine(start, position);
                }

                start = position;
            }
        }
    }

    public class IndicatorGaugeLaggingProxy : IndicatorGaugeProxy
    {
        public float updateThreshold = 0.001f;
        public float smoothTime = 0.5f;
    }
}
