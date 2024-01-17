using AutoMapper;
using CCL.Types.Proxies;

namespace CCL.Importer.Proxies
{
    internal class MiscPrivateReplacer : Profile
    {
        public MiscPrivateReplacer()
        {
            ShouldMapField = f => f.IsPublic | f.IsPrivate;

            CreateMap<ExplosionModelHandlerProxy, ExplosionModelHandler>().CacheAndProcessProxyAutomatically();
        }
    }
}
