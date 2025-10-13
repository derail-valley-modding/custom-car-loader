using UnityEngine;

namespace CCL.Types.Proxies.Controls.VR
{
    [AddComponentMenu("CCL/Proxies/Controls/VR/Speed Zone Control Touch Behaviour Proxy")]
    public class SpeedZoneControlTouchBehaviourProxy : MonoBehaviour
    {
        public Transform direction = null!;
        public bool onlyDoForwardDirection;
        public bool useOnUntouch;

        private void OnDrawGizmos()
        {
            if (direction == null) return;

            using (new GizmoUtil.MatrixScope(direction.localToWorldMatrix))
            {
                Gizmos.color = Color.cyan;

                var up = Vector3.up * 0.025f;
                var side = Vector3.right * 0.025f;
                Gizmos.DrawLine(up, side);
                Gizmos.DrawLine(up, -side);
                Gizmos.DrawLine(Vector3.zero, -up + side);
                Gizmos.DrawLine(Vector3.zero, -up - side);
            }
        }
    }
}
