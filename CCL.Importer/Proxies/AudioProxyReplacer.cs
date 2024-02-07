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
            CreateMap<DopplerProxy, Doppler>().AutoCacheAndMap();
            CreateMap<AudioLayerProxy, LayeredAudio.Layer>();
            CreateMap<LayeredAudioProxy, LayeredAudio>().AutoCacheAndMap()
                .AfterMap(DestroyLayerProxies);

            CreateMap<AudioClipPortReaderProxy, AudioClipPortReader>().AutoCacheAndMap();
            CreateMap<LayeredAudioPortReaderProxy, LayeredAudioPortReader>().AutoCacheAndMap();
        }

        private static void DestroyLayerProxies(LayeredAudioProxy proxy, LayeredAudio _)
        {
            if (proxy.layers == null) return;

            foreach (var layerProxy in proxy.layers)
            {
                Object.Destroy(layerProxy);
            }
        }
    }
}
