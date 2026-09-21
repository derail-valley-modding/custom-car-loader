using CCL.Types.Proxies.Wheels;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Components.Wheels
{
    [AddComponentMenu("CCL/Components/Wheels/Extra Powered Wheel Rotation Via Code")]
    public class ExtraPoweredWheelRotationViaCode : PoweredWheelRotationBaseProxy, ISelfValidation
    {
        private PoweredWheelsManagerProxy? _cachedManager;

        public SelfValidationResult Validate(out string message, out string? highlight)
        {
            TryToGetManager();

            if (_cachedManager != null)
            {
                if (_cachedManager.poweredWheels.Any(x => x != null && x.wheelTransform == null))
                {
                    message = $"{nameof(PoweredWheelProxy)} must have a transform when using {nameof(ExtraPoweredWheelRotationViaCode)}";
                    highlight = null;
                    return SelfValidationResult.Fail;
                }
            }

            return this.Pass(out message, out highlight);
        }

        private void OnDrawGizmos()
        {
            TryToGetManager();

            if (_cachedManager != null)
            {
                foreach (var item in _cachedManager.poweredWheels)
                {
                    if (item.wheelTransform != null)
                    {
                        DrawWheelGizmo(item.wheelTransform, item.localRotationAxis, wheelRadius, true);
                    }
                }
            }
        }

        private void TryToGetManager()
        {
            if (_cachedManager == null)
            {
                _cachedManager = transform.root.GetComponentInChildren<PoweredWheelsManagerProxy>();
            }
        }
    }
}
