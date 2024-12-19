using AutoMapper;
using CCL.Types.Proxies.Util;
using DV.Util;

namespace CCL.Importer.Proxies.Util
{
    internal class UtilReplacer : Profile
    {
        public UtilReplacer()
        {
            CreateMap<PositionSyncProviderProxy, PositionSyncProvider>().AutoCacheAndMap();
            CreateMap<PositionSyncConsumerProxy, PositionSyncConsumer>().AutoCacheAndMap();
        }
    }
}
