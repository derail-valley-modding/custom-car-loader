using CCL.Types.Proxies.Controllers;
using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    [AddComponentMenu("CCL/Proxies/Weather/Wipers Sim Control Input Proxy")]
    public class WipersSimControlInputProxy : PoweredControlHandlerBase
    {
        public WiperControllerProxy wiperController = null!;
    }
}
