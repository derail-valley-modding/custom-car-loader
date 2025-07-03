using UnityEngine;

namespace CCL.Types.Proxies.Controls.VR
{
    [AddComponentMenu("CCL/Proxies/Controls/VR/Speed Zone Control Touch Behaviour Proxy")]
    public class SpeedZoneControlTouchBehaviourProxy : MonoBehaviour
    {
        public Transform direction = null!;
        public bool onlyDoForwardDirection;
        public bool useOnUntouch;
    }
}
