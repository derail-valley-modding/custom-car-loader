using AutoMapper;
using CCL.Types.Proxies.Controllers;
using DV.Damage;
using DV.Simulation.Brake;

namespace CCL.Importer.Proxies.Controllers
{
    internal class MiscControllerReplacer : Profile
    {
        public MiscControllerReplacer()
        {
            CreateMap<DamageControllerProxy, DamageController>().AutoCacheAndMap();
            CreateMap<CompressorSimControllerProxy, CompressorSimController>().AutoCacheAndMap();
        }
    }
}
