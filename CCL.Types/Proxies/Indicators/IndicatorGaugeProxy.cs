using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    [AddComponentMenu("CCL/Proxies/Indicators/Indicator Gauge Proxy")]
    public class IndicatorGaugeProxy : IndicatorProxy, ISelfValidation
    {
        public Transform needle = null!;
        public float minAngle = -180f;
        public float maxAngle = 180f;
        public bool unclamped;
        public Vector3 rotationAxis = Vector3.back;

        [Header("Editor visualization")]
        public float gizmoRadius = 0.1f;
        public float angleOffset = 0;

        public SelfValidationResult Validate(out string message)
        {
            if (needle == null)
            {
                return this.FailForNull(nameof(needle), out message);
            }

            return this.Pass(out message);
        }

        private void OnDrawGizmos()
        {
            if (!needle) return;

            GizmoUtil.DrawLocalRotationArc(transform, minAngle, maxAngle, rotationAxis, START_COLOR, END_COLOR, MID_COLOR, gizmoRadius, angleOffset);
        }
    }

    [AddComponentMenu("CCL/Proxies/Indicators/Indicator Gauge Lagging Proxy")]
    public class IndicatorGaugeLaggingProxy : IndicatorGaugeProxy
    {
        [Header("Smooth properties")]
        public float updateThreshold = 0.001f;
        public float smoothTime = 0.5f;
    }
}
