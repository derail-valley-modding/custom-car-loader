using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    public class WheelRotationViaCodeProxy : WheelRotationBaseProxy
    {
        public Vector3 rotationAxis = Vector3.right;
        public Transform[] transformsToRotate = new Transform[0];
    }
}
