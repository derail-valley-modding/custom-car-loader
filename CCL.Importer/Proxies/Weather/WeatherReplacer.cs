using AutoMapper;
using CCL.Types.Proxies.Weather;
using DV.Openables;
using DV.Rain;
using PlaceholderSoftware.WetStuff;

namespace CCL.Importer.Proxies.Weather
{
    internal class WeatherReplacer : Profile
    {
        public WeatherReplacer()
        {
            CreateMap<WindowProxy, Window>().AutoCacheAndMap()
                .ForMember(d => d.duplicates, o => o.MapFrom(s => Mapper.GetFromCache(s.duplicates)))
                .ForMember(d => d.wipers, o => o.MapFrom(s => Mapper.GetFromCache(s.wipers)));
            CreateMap<CabinDryVolumeProxy, CabinDryVolume>().AutoCacheAndMap()
                .ForMember(d => d.subVolumes, o => o.MapFrom(s => Mapper.GetFromCache(s.subVolumes)));

            CreateMap<WetDecalProxy, WetDecal>().AutoCacheAndMap();

            CreateMap<DecalSettingsProxy, DecalSettings>();
            CreateMap<DecalLayerProxy, DecalLayer>();
            CreateMap<DecalLayerChannelProxy, DecalLayerChannel>();

            CreateMap<OpenableControlProxy, OpenableControl>().AutoCacheAndMap();

            CreateMap<WipersSimControlInputProxy, WipersSimControlInput>().AutoCacheAndMap()
                .ForMember(d => d.wiperController, o => o.MapFrom(s => Mapper.GetFromCache(s.wiperController)));
            CreateMap<WiperControllerProxy, WiperController>().AutoCacheAndMap()
                .ForMember(d => d.wiperDrivers, o => o.MapFrom(s => Mapper.GetFromCache(s.wiperDrivers)));
            CreateMap<WiperProxy, Wiper>().AutoCacheAndMap()
                .ForMember(d => d.windows, o => o.MapFrom(s => Mapper.GetFromCache(s.windows)));
            CreateMap<WiperDriverProxy, WiperDriver>()
                .ForMember(d => d.master, o => o.MapFrom(s => Mapper.GetFromCache(s.master)))
                .ForMember(d => d.wiper, o => o.MapFrom(s => Mapper.GetFromCache(s.wiper)))
                .IncludeAllDerived();
            CreateMap<RotaryWiperDriverProxy, RotaryWiperDriver>().AutoCacheAndMap();
        }
    }
}
