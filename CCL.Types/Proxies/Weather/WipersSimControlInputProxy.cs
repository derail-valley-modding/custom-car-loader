using CCL.Types.Proxies.Controllers;
using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    [AddComponentMenu("CCL/Proxies/Weather/Wipers Sim Control Input Proxy")]
    public class WipersSimControlInputProxy : PoweredControlHandlerBase, ISelfValidation
    {
        public WiperControllerProxy wiperController = null!;

        public SelfValidationResult Validate(out string message)
        {
            if (wiperController == null)
            {
                return this.FailForNull(nameof(wiperController), out message);
            }

            message = $"Make sure both the {nameof(WiperControllerProxy.speeds)} and {nameof(WiperControllerProxy.timeBetweenWipes)} arrays " +
                $"have the same number of entries as the values for {controlId}";
            return SelfValidationResult.Info;
        }
    }
}
