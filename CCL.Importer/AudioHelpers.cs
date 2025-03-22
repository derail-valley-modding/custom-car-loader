using CCL.Types.Proxies.Audio;
using DV.Simulation.Ports;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CCL.Importer
{
    public static class AudioHelpers
    {
        private static Dictionary<DVAudioMixerGroup, AudioMixerGroup> s_mixerGroups = new Dictionary<DVAudioMixerGroup, AudioMixerGroup>();
        private static GameObject? s_audioS282;
        private static GameObject? s_audioDE6;

        private static GameObject AudioS282 => Extensions.GetCached(ref s_audioS282,
            () => QuickAccess.Locomotives.S282A.parentType.audioPrefab);
        private static GameObject AudioDE6 => Extensions.GetCached(ref s_audioDE6,
            () => QuickAccess.Locomotives.DE6.parentType.audioPrefab);

        public static AudioMixerGroup GetMixerGroup(DVAudioMixerGroup group)
        {
            if (s_mixerGroups.TryGetValue(group, out AudioMixerGroup mixer))
            {
                return mixer;
            }

            mixer = group switch
            {
                DVAudioMixerGroup.Airflow => AudioS282.transform.Find("BrakeModule/Airflow/AirflowLayers")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixerGroup.Brake => AudioS282.transform.Find("BrakeModule/Brake/Brake_Layered")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixerGroup.Cab => AudioDE6.transform.Find("[sim] Engine/CabFan_Layered")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixerGroup.Collisions => AudioS282.transform.Find("[sim] Engine/CylinderCrack/CylinderCrackAudioClip")
                    .GetComponent<AudioClipPortReader>().mixerGroup,
                DVAudioMixerGroup.Compressor => AudioDE6.transform.Find("[sim] Engine/Compressor_Layered")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixerGroup.Chuffs => AudioS282.transform.Find("[sim] Engine/SteamChuff/2ChuffsPerSecond")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixerGroup.Derailment => AudioS282.transform.Find("CarBaseAudioModules/DerailLayers")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixerGroup.Engine => AudioDE6.transform.Find("[sim] Engine/ElectricMotor_Layered")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixerGroup.Horn => AudioS282.transform.Find("[sim] Engine/Whistle/Whistle_Layered")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixerGroup.Master => AudioS282.transform.Find("[sim] Engine/CoalDump/SandFlowLayers")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixerGroup.Outdoors => AudioS282.transform.Find("[sim] Engine/Fire/Fire_Layered")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                DVAudioMixerGroup.Wheels => AudioS282.transform.Find("WheelsModule/WheelsLeft/WheelslipLayers")
                    .GetComponent<LayeredAudio>().audioMixerGroup,
                _ => throw new System.NotImplementedException(nameof(group))
            };

            s_mixerGroups.Add(group, mixer);
            return mixer;
        }
    }
}