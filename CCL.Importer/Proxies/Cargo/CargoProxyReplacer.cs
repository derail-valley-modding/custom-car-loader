using AutoMapper;
using CCL.Types.Proxies.Cargo;
using DV.Cargo;

namespace CCL.Importer.Proxies.Cargo
{
    internal class CargoProxyReplacer : Profile
    {
        public CargoProxyReplacer()
        {
            CreateMap<CargoBoundsProxy, CargoBounds>().AutoCacheAndMap();
            CreateMap<CargoWaterDamageProxy, CargoWaterDamage>().AutoCacheAndMap();
            CreateMap<CargoReactionToDamageProxy, CargoReactionToDamage>().AutoCacheAndMap();
        }
    }
}
