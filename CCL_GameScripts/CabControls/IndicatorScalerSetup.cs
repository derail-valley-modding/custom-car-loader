using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public class IndicatorScalerSetup : IndicatorSetupBase
    {
        public override string TargetTypeName => "IndicatorScaler";

        [ProxyField]
        public Transform indicatorToScale;

        [ProxyField]
        public Vector3 startScale = Vector3.one;
        [ProxyField]
        public Vector3 endScale = Vector3.one;

        public void OnDrawGizmos()
        {
            if (!indicatorToScale) return;

            Vector3 worldCenter = ModelUtil.GetModelCenterline(gameObject);
            Vector3 localCenter = transform.InverseTransformPoint(worldCenter);
            Vector3 worldStart = transform.TransformPoint(Vector3.Scale(localCenter, startScale));
            Vector3 worldEnd = transform.TransformPoint(Vector3.Scale(localCenter, endScale));

            float tickScale = Vector3.Distance(worldStart, worldEnd) * 0.1f;
            Vector3 axis = (worldEnd - worldStart).normalized;
            Vector3 perpendicular = (axis.z < axis.x) ? new Vector3(axis.y, -axis.x, 0) : new Vector3(0, -axis.z, axis.y);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(worldStart, worldEnd);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(worldStart + perpendicular * tickScale, worldStart + perpendicular * -tickScale);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(worldEnd + perpendicular * tickScale, worldEnd + perpendicular * -tickScale);
        }
    }
}
