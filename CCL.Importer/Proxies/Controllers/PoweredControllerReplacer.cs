using CCL.Types.Proxies.Controllers;
using DV.Simulation.Cars;

namespace CCL.Importer.Proxies.Controllers
{
    [ProxyMap(typeof(CabLightsControllerProxy), typeof(CabLightsController))]
    public class PoweredControllerReplacer : ProxyReplacer
    {
    }
}
