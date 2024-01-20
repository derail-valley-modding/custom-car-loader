using AutoMapper;
using CCL.Types.Proxies.Weather;
using DV.Rain;
using PlaceholderSoftware.WetStuff;

namespace CCL.Importer.Proxies.Weather
{
    internal class WeatherReplacer : Profile
    {
        public WeatherReplacer()
        {
            CreateMap<WindowProxy, Window>()
                .AutoCacheAndMap()
                .ForMember(d => d.duplicates, o => o.MapFrom(s => Mapper.GetFromCache(s.duplicates)));
            CreateMap<CabinDryVolumeProxy, CabinDryVolume>().AutoCacheAndMap()
                .ForMember(d => d.subVolumes, o => o.MapFrom(s => Mapper.GetFromCache(s.subVolumes)));
        }
    }
}
