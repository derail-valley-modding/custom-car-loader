using AutoMapper;
using CCL.Types.Proxies.Controls;
using DV.HUD;

namespace CCL.Importer.Proxies.Controls
{
    internal class HUDElementReplacer : Profile
    {
        public HUDElementReplacer()
        {
            CreateMap<NotchedControlCustomNamesProxy, NotchedControlCustomNames>().AutoCacheAndMap();
            CreateMap<NotchedControlNumberedNamesProxy, NotchedControlNumberedNames>().AutoCacheAndMap();
            CreateMap<PercentageControlNamesProxy, PercentageControlNames>().AutoCacheAndMap();

            CreateMap<LocoControlsReaderProxy, LocoControlsReader>().AutoCacheAndMap();
            CreateMap<LocoIndicatorReaderProxy, LocoIndicatorReader>().AutoCacheAndMap()
                .ForAllMembers(o => o.MapFrom(m => Mapper.GetFromCache(m)));
            CreateMap<LocoLampReaderProxy, LocoLampReader>().AutoCacheAndMap()
                .ForAllMembers(o => o.MapFrom(m => Mapper.GetFromCache(m)));
        }
    }
}
