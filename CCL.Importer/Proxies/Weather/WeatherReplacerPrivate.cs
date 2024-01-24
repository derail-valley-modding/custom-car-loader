using AutoMapper;
using CCL.Types.Proxies.Weather;
using PlaceholderSoftware.WetStuff;

namespace CCL.Importer.Proxies.Weather
{
    internal class WeatherReplacerPrivate : Profile
    {
        public WeatherReplacerPrivate()
        {
            ShouldMapField = f => AutoMapperHelper.IsPublicOrSerialized(f);

            CreateMap<WetDecalProxy, WetDecal>().AutoCacheAndMap();

            CreateMap<DecalSettingsProxy, DecalSettings>();
            CreateMap<DecalLayerProxy, DecalLayer>();
            CreateMap<DecalLayerChannelProxy, DecalLayerChannel>();
        }
    }
}
