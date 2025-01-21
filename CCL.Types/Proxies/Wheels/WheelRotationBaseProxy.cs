using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    public abstract class WheelRotationBaseProxy : MonoBehaviour
    {
        public float wheelRadius = 0.7f;
        public bool affectedByWheelSlide = true;
    }
}
