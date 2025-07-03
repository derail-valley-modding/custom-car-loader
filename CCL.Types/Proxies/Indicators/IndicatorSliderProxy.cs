using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    [AddComponentMenu("CCL/Proxies/Indicators/Indicator Slider Proxy")]
    public class IndicatorSliderProxy : IndicatorProxy
    {
        public Transform pointer = null!;
        public Vector3 startPosition = -Vector3.right;
        public Vector3 endPosition = Vector3.right;

        public void OnDrawGizmos()
        {
            if (!pointer) return;

            Vector3 worldStart = pointer.parent.TransformPoint(startPosition);
            Vector3 worldEnd = pointer.parent.TransformPoint(endPosition);

            float tickScale = Vector3.Distance(startPosition, endPosition) * 0.1f;
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
