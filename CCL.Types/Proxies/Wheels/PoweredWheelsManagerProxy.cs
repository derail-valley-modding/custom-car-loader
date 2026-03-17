using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    [AddComponentMenu("CCL/Proxies/Wheels/Powered Wheels Manager Proxy")]
    public class PoweredWheelsManagerProxy : MonoBehaviour, ISelfValidation
    {
        public PoweredWheelProxy[] poweredWheels = new PoweredWheelProxy[0];
        public bool GetWheelsFromDefaultBogies = true;

        public SelfValidationResult Validate(out string message, out string? highlight)
        {
            if (poweredWheels.Length > 255)
            {
                message = "powered wheels cannot be more than 255";
                highlight = nameof(poweredWheels);
                return SelfValidationResult.Fail;
            }

            return this.Pass(out message, out highlight);
        }
    }

    [AddComponentMenu("CCL/Proxies/Wheels/Powered Wheel Proxy")]
    public class PoweredWheelProxy : MonoBehaviour, ISelfValidation
    {
        public Transform wheelTransform = null!;
        public Vector3 localRotationAxis = Vector3.right;

        public SelfValidationResult Validate(out string message, out string? highlight)
        {
            if (wheelTransform == null)
            {
                return this.FailForNull(nameof(wheelTransform), out message, out highlight);
            }

            return this.Pass(out message, out highlight);
        }
    }
}
