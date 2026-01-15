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
    }
}
