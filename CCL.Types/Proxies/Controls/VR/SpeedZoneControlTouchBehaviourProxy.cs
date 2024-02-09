using UnityEngine;

namespace CCL.Types.Proxies.Controls.VR
{
    public class SpeedZoneControlTouchBehaviourProxy : MonoBehaviour
    {
        public Transform direction = null!;
        public bool onlyDoForwardDirection;
        public bool useOnUntouch;
    }
}
