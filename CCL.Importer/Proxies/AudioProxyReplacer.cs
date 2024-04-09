using AutoMapper;
using CCL.Types.Proxies.Audio;
using DV.DopplerEffects;
using DV.ModularAudioCar;
using DV.Simulation.Ports;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace CCL.Importer.Proxies
{
    internal class AudioProxyReplacer : Profile
    {
        private static Dictionary<DVAudioMixGroup, AudioMixerGroup> s_mixerGroups = new Dictionary<DVAudioMixGroup, AudioMixerGroup>();
        private static GameObject? s_audioS282;
        private static GameObject? s_audioDE6;

        private static GameObject AudioS282 => Extensions.GetCached(ref s_audioS282,
            () => TrainCarType.LocoSteamHeavy.ToV2().parentType.audioPrefab);
        private static GameObject AudioDE6 => Extensions.GetCached(ref s_audioDE6,
            () => TrainCarType.LocoDiesel.ToV2().parentType.audioPrefab);

        public AudioProxyReplacer()
        {
            CreateMap<DopplerProxy, Doppler>().AutoCacheAndMap()
                .AfterMap(EliminateSourceWithPrejudice);
            CreateMap<AudioLayerProxy, LayeredAudio.Layer>()
                .AfterMap(EliminateSourceWithPrejudice);
            CreateMap<LayeredAudioProxy, LayeredAudio>().AutoCacheAndMap()
                .ForMember(d => d.audioMixerGroup, o => o.Ignore())
                .AfterMap(LayeredAudioAfter);

            CreateMap<AudioClipPortReaderProxy, AudioClipPortReader>().AutoCacheAndMap();
            CreateMap<LayeredAudioPortReaderProxy, LayeredAudioPortReader>().AutoCacheAndMap();

            CreateMap<CylinderCockLayeredPortReaderProxy, CylinderCockLayeredPortReader>().AutoCacheAndMap()
                .ReplaceInstancedObjects()
                .ForMember(d => d.cylCockAudio, o => o.MapFrom(s => s.cylCockAudio.Select(x => x.GetComponent<LayeredAudio>())));
        }

        private static void EliminateSourceWithPrejudice<TSrc, TDest>(TSrc obj, TDest _)
            where TSrc : Object
        {
            Object.DestroyImmediate(obj);
        }

        private void LayeredAudioAfter(LayeredAudioProxy src, LayeredAudio dest)
        {
            dest.audioMixerGroup = GetMixerGroup(src.audioMixGroup);
        }

        private static AudioMixerGroup GetMixerGroup(DVAudioMixGroup group)
        {
            if (s_mixerGroups.TryGetValue(group, out AudioMixerGroup mixer))
            {
                return mixer;
            }

            mixer = group switch
            {
                DVAudioMixGroup.Airflow => AudioS282.transform.Find("BrakeModule/Airflow/AirflowLayers")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixGroup.Brake => AudioS282.transform.Find("BrakeModule/Brake/Brake_Layered")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixGroup.Cab => AudioDE6.transform.Find("[sim] Engine/CabFan_Layered")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixGroup.Collisions => AudioS282.transform.Find("[sim] Engine/CylinderCrack/CylinderCrackAudioClip")
                    .GetComponent<AudioClipPortReader>().mixerGroup,
                DVAudioMixGroup.Chuffs => AudioS282.transform.Find("[sim] Engine/SteamChuff/2ChuffsPerSecond")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixGroup.Derailment => AudioS282.transform.Find("CarBaseAudioModules/DerailLayers")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixGroup.Engine => AudioDE6.transform.Find("[sim] Engine/ElectricMotor_Layered")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixGroup.Horn => AudioS282.transform.Find("[sim] Engine/Whistle/Whistle_Layered")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixGroup.Master => AudioS282.transform.Find("[sim] Engine/CoalDump/SandFlowLayers")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixGroup.Outdoors => AudioS282.transform.Find("[sim] Engine/Fire/Fire_Layered")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixGroup.Wheels => AudioS282.transform.Find("WheelsModule/WheelsLeft/WheelslipLayers")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                _ => throw new System.NotImplementedException(nameof(group))
            };

            s_mixerGroups.Add(group, mixer);
            return mixer;
        }
    }
}
