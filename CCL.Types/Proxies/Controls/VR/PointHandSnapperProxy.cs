using UnityEngine;

namespace CCL.Types.Proxies.Controls.VR
{
    [AddComponentMenu("CCL/Proxies/Controls/VR/Point Hand Snapper Proxy")]
    public class PointHandSnapperProxy : AHandPoseSnapperProxy
    {
        [Tooltip("Position is used if an object is provided here, rotation is irrelevant. If nothing is assigned, this GameObject's transform will be used instead")]
        public Transform pointMarker = null!;

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere((pointMarker ? pointMarker : transform).position, 0.01f);
        }
    }
}
