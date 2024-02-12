using UnityEngine;
using UnityEngine.Animations;

namespace CCL.Types.Proxies.Wheels
{
    public class WheelRotationViaCodeProxy : WheelRotationBaseProxy
    {
        public Axis rotationAxis;
        public Transform[] transformsToRotate = new Transform[0];
    }
}
