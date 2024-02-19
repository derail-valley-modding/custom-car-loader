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
        }
    }
}
