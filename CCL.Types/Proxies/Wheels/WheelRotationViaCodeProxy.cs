using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    [AddComponentMenu("CCL/Proxies/Wheels/Wheel Rotation Via Code Proxy")]
    public class WheelRotationViaCodeProxy : WheelRotationBaseProxy
    {
        public Axis rotationAxis;
        public Transform[] transformsToRotate = new Transform[0];

        private void OnDrawGizmos()
        {
            var axis = ToVector(rotationAxis);

            foreach (var item in transformsToRotate)
            {
                if (item != null)
                {
                    DrawWheelGizmo(item, axis, wheelRadius, false);
                }
            }
        }

        private static Vector3 ToVector(Axis axis) => axis switch
        {
            Axis.X => Vector3.right,
            Axis.Y => Vector3.up,
            Axis.Z => Vector3.forward,
            _ => Vector3.zero
        };
    }
}
