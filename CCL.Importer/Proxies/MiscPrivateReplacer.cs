using AutoMapper;
using CCL.Types.Proxies;
using CCL.Types.Proxies.Weather;
using PlaceholderSoftware.WetStuff;

namespace CCL.Importer.Proxies
{
    internal class MiscPrivateReplacer : Profile
    {
        public MiscPrivateReplacer()
        {
            ShouldMapField = f => f.IsPublic | f.IsPrivate;

            CreateMap<ExplosionModelHandlerProxy, ExplosionModelHandler>().AutoCacheAndMap();
            CreateMap<ExplosionModelHandlerProxy.MaterialSwapData, ExplosionModelHandler.MaterialSwapData>();
            CreateMap<ExplosionModelHandlerProxy.GameObjectSwapData, ExplosionModelHandler.GameObjectSwapData>();
        }
    }
}
