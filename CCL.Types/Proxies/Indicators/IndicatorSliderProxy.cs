using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    [AddComponentMenu("CCL/Proxies/Indicators/Indicator Slider Proxy")]
    public class IndicatorSliderProxy : IndicatorProxy
    {
        public Transform pointer = null!;
        public Vector3 startPosition = -Vector3.right;
        public Vector3 endPosition = Vector3.right;

        [RenderMethodButtons, SerializeField]
        [MethodButton(nameof(CopyStart), "Copy Current To Start")]
        [MethodButton(nameof(CopyEnd), "Copy Current To End")]
        [MethodButton(nameof(PreviewStart), "Preview Start")]
        [MethodButton(nameof(PreviewEnd), "Preview End")]
        private bool _buttons;

        private void OnDrawGizmos()
        {
            if (!pointer) return;

            Vector3 worldStart = pointer.parent.TransformPoint(startPosition);
            Vector3 worldEnd = pointer.parent.TransformPoint(endPosition);

            float tickScale = Vector3.Distance(startPosition, endPosition) * 0.1f;
            Vector3 axis = (worldEnd - worldStart).normalized;
            Vector3 perpendicular = (axis.z < axis.x) ? new Vector3(axis.y, -axis.x, 0) : new Vector3(0, -axis.z, axis.y);

            Gizmos.color = MID_COLOR;
            Gizmos.DrawLine(worldStart, worldEnd);
            Gizmos.color = START_COLOR;
            Gizmos.DrawLine(worldStart + perpendicular * tickScale, worldStart + perpendicular * -tickScale);
            Gizmos.color = END_COLOR;
            Gizmos.DrawLine(worldEnd + perpendicular * tickScale, worldEnd + perpendicular * -tickScale);
        }

        private void CopyStart()
        {
            if (pointer == null) return;

            startPosition = pointer.localPosition;
        }

        private void CopyEnd()
        {
            if (pointer == null) return;

            endPosition = pointer.localPosition;
        }
        private void PreviewStart()
        {
            if (pointer == null) return;

            pointer.localPosition = startPosition;
        }
        private void PreviewEnd()
        {
            if (pointer == null) return;

            pointer.localPosition = endPosition;
        }
    }
}
