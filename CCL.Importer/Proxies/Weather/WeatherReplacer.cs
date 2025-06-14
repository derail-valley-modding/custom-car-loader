using AutoMapper;
using CCL.Types;
using CCL.Types.Proxies.Weather;
using DV.Openables;
using DV.Rain;
using PlaceholderSoftware.WetStuff;
using UnityEngine;

namespace CCL.Importer.Proxies.Weather
{
    internal class WeatherReplacer : Profile
    {
        private static WiperAudio? s_wiperAudioDE6;
        private static WiperAudio? s_wiperAudioS060;
        private static WiperAudio WiperAudioDE6 => Extensions.GetCached(ref s_wiperAudioDE6,
            () => QuickAccess.Locomotives.DE6.prefab.GetComponentInChildren<WiperAudio>());
        private static WiperAudio WiperAudioS060 => Extensions.GetCached(ref s_wiperAudioS060,
            () => QuickAccess.Locomotives.S060.prefab.GetComponentInChildren<WiperAudio>());

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

            CreateMap<WiperAudioProxy, WiperAudio>().AutoCacheAndMap()
                .ForMember(d => d.wiperController, o => o.MapFrom(s => Mapper.GetFromCache(s.wiperController)))
                .ForMember(d => d.driver, o => o.MapFrom(s => Mapper.GetFromCache(s.driver)));
            CreateMap<SimpleWiperAudio, WiperAudio>().AutoCacheAndMap()
                .ForMember(d => d.wiperController, o => o.MapFrom(s => Mapper.GetFromCache(s.wiperController)))
                .ForMember(d => d.driver, o => o.MapFrom(s => Mapper.GetFromCache(s.driver)))
                .AfterMap(SimpleWiperAudioAfter);
        }

        private void SimpleWiperAudioAfter(SimpleWiperAudio proxy, WiperAudio audio)
        {
            var source = proxy.Type switch
            {
                SimpleWiperAudio.WiperType.Electric => WiperAudioDE6,
                SimpleWiperAudio.WiperType.Pneumatic => WiperAudioS060,
                _ => null!,
            };

            audio.slideVolumeCurve = source.slideVolumeCurve;
            audio.wetClip = source.wetClip;
            audio.dryClip = source.dryClip;
            audio.motorClip = source.motorClip;

            audio.leftMove = source.leftMove;
            audio.rightMove = source.rightMove;

            audio.slideAudioSource = Object.Instantiate(source.slideAudioSource, audio.transform);
            audio.motorAudioSource = Object.Instantiate(source.motorAudioSource, audio.transform);
            audio.endAudio = Object.Instantiate(source.endAudio, audio.transform);

            audio.slideAudioSource.transform.ResetLocal();
            audio.motorAudioSource.transform.ResetLocal();
            audio.endAudio.transform.ResetLocal();

            audio.slideVolume = source.slideVolume;
            audio.endClip = source.endClip;
            audio.endVolume = source.endVolume;
            audio.endPitchLeft = source.endPitchLeft;
            audio.endPitchRight = source.endPitchRight;
        }
    }
}
