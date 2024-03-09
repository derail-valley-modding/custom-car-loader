using UnityEngine;

namespace CCL.Types.Proxies.Controls.VR
{
    public class DoorClosedVRTouchDisableProxy : MonoBehaviour
    {
        public GameObject door = null!;
        public float closedValue;
        public float threshold = 0.1f;
        public bool offWhenClosed = true;
    }
}
