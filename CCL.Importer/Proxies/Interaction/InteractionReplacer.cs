using AutoMapper;
using CCL.Types.Proxies.Interaction;
using DV.Interaction;

namespace CCL.Importer.Proxies.Interaction
{
    internal class InteractionReplacer : Profile
    {
        public InteractionReplacer()
        {
            CreateMap<NonPhysicsCoalTargetProxy, NonPhysicsCoalTarget>().AutoCacheAndMap();

            CreateMap<ItemUseTargetProxy, ItemUseTarget>().AutoCacheAndMap();
            CreateMap<ItemUseRedirectProxy, ItemUseRedirect>().AutoCacheAndMap()
                .ForMember(d => d.target, o => o.MapFrom(s => Mapper.GetFromCache(s.target)));
        }
    }
}
