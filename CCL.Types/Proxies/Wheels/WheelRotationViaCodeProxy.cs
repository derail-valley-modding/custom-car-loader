using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    [AddComponentMenu("CCL/Proxies/Wheels/Wheel Rotation Via Code Proxy")]
    public class WheelRotationViaCodeProxy : WheelRotationBaseProxy
    {
        public Axis rotationAxis;
        public Transform[] transformsToRotate = new Transform[0];
    }
}
