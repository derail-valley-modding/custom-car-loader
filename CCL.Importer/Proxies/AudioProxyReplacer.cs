using AutoMapper;
using CCL.Types.Proxies.Audio;
using DV.DopplerEffects;
using DV.ModularAudioCar;
using DV.Simulation.Ports;
using System.Linq;
using UnityEngine;

namespace CCL.Importer.Proxies
{
    internal class AudioProxyReplacer : Profile
    {
        public AudioProxyReplacer()
        {
            CreateMap<DopplerProxy, Doppler>().AutoCacheAndMap()
                .AfterMap(EliminateSourceWithPrejudice);
            CreateMap<AudioLayerProxy, LayeredAudio.Layer>()
                .AfterMap(EliminateSourceWithPrejudice);
            CreateMap<LayeredAudioProxy, LayeredAudio>().AutoCacheAndMap()
                .ForMember(d => d.audioMixerGroup, o => o.MapFrom(s => AudioHelpers.GetMixerGroup(s.audioMixGroup)));

            CreateMap<AudioClipPortReaderProxy, AudioClipPortReader>().AutoCacheAndMap()
                .ForMember(d => d.mixerGroup, o => o.MapFrom(s => s.audioMixGroup.ToInstance()));
            CreateMap<LayeredAudioPortReaderProxy, LayeredAudioPortReader>().AutoCacheAndMap();

            CreateMap<CylinderCockLayeredPortReaderProxy, CylinderCockLayeredPortReader>().AutoCacheAndMap()
                .ReplaceInstancedObjects()
                .ForMember(d => d.cylCockAudio, o => o.MapFrom(s => s.cylCockAudio.Select(x => x.GetComponent<LayeredAudio>())));
            CreateMap<ChuffClipsSimReaderProxy, ChuffClipsSimReader>().AutoCacheAndMap()
                .ReplaceInstancedObjects();
            CreateMap<ChuffLoop, ChuffClipsSimReader.ChuffLoop>()
                .ForMember(d => d.chuffLoop, o => o.MapFrom(s => s.chuffLoop.GetComponent<LayeredAudio>()))
                .AfterMap(EliminateSourceWithPrejudice);
            CreateMap<OrderedChuffClips, ChuffClipsSimReader.OrderedChuffClips>()
                .AfterMap(EliminateSourceWithPrejudice);
            CreateMap<IndividualChuffAudioSourceConfig, ChuffClipsSimReader.IndividualChuffAudioSourceConfig>()
                .AfterMap(EliminateSourceWithPrejudice);
        }

        private static void EliminateSourceWithPrejudice<TSrc, TDest>(TSrc obj, TDest _)
            where TSrc : Object
        {
            Object.DestroyImmediate(obj);
        }
    }
}
