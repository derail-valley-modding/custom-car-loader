using AutoMapper;
using CCL.Types.Proxies.Audio;
using DV.DopplerEffects;
using DV.Simulation.Ports;
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
            CreateMap<LayeredAudioProxy, LayeredAudio>().AutoCacheAndMap();

            CreateMap<AudioClipPortReaderProxy, AudioClipPortReader>().AutoCacheAndMap();
            CreateMap<LayeredAudioPortReaderProxy, LayeredAudioPortReader>().AutoCacheAndMap();
        }

        private static void EliminateSourceWithPrejudice<TSrc, TDest>(TSrc obj, TDest _)
            where TSrc : Object
        {
            Object.DestroyImmediate(obj);
        }
    }
}
