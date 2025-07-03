using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    [AddComponentMenu("CCL/Proxies/Wheels/Powered Wheels Manager Proxy")]
    public class PoweredWheelsManagerProxy : MonoBehaviour
    {
        public PoweredWheelProxy[] poweredWheels = new PoweredWheelProxy[0];
        public bool GetWheelsFromDefaultBogies = true;
    }

    [AddComponentMenu("CCL/Proxies/Wheels/Powered Wheel Proxy")]
    public class PoweredWheelProxy : MonoBehaviour
    {
        public Transform wheelTransform = null!;
        public Vector3 localRotationAxis = Vector3.right;
    }
}
