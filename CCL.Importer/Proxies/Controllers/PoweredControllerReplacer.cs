using AutoMapper;
using CCL.Types.Proxies.Controllers;
using DV.Simulation.Cars;

namespace CCL.Importer.Proxies.Controllers
{
    public class PoweredControllerReplacer : Profile
    {
        public PoweredControllerReplacer()
        {
            CreateMap<CabLightsControllerProxy, CabLightsController>().AutoCacheAndMap();
        }
    }
}
