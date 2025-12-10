using UnityEngine;

namespace CCL.Types.Proxies.Controls.VR
{
    [AddComponentMenu("CCL/Proxies/Controls/VR/Line Hand Snapper Proxy")]
    public class LineHandSnapperProxy : AHandPoseSnapperProxy
    {
        public Transform lineStart = null!;
        [Tooltip("This extends in the Y axis of the start transform")]
        public float lineLength = 1f;
        public float minAngle;
        public float maxAngle = 360f;

        public void OnDrawGizmosSelected()
        {
            if (lineStart != null)
            {
                Vector3 position = lineStart.position;
                Vector3 vector = lineStart.TransformPoint(Vector3.up * lineLength);
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(position, vector);
                Gizmos.DrawSphere(position, 0.01f);
                Gizmos.DrawSphere(vector, 0.01f);
            }
        }
    }
}
