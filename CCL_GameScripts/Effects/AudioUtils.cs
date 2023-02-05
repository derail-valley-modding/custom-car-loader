using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CCL_GameScripts.Effects
{
    public enum DVAudioMixerGroup
    {
        Master,
        Air_Pipe,
        Boombox,
        Brake,
        Cab,
        Collisions,
        Derailment,
        Engine,
        Explosions,
        External,
        Joints,
        Horn,
        Rail,
        Squeal,
        Wheels,
        Wind,
    }

    public static class AudioUtils
    {
        private static Dictionary<DVAudioMixerGroup, AudioMixerGroup> mixerCache = new Dictionary<DVAudioMixerGroup, AudioMixerGroup>();

        public static void Initialize()
        {
            var mixers = Resources.FindObjectsOfTypeAll<AudioMixerGroup>();

            foreach (var mixer in mixers)
            {
                if (Enum.TryParse(mixer.name.Replace(' ', '_'), out DVAudioMixerGroup groupId))
                {
                    mixerCache[groupId] = mixer;
                }
            }
        }

        public static AudioMixerGroup GetMixerGroup(DVAudioMixerGroup id)
        {
            if (mixerCache.TryGetValue(id, out AudioMixerGroup mixer))
            {
                return mixer;
            }
            return null;
        }

        private static AnimationCurve resetCurve = new AnimationCurve(new Keyframe[]
        {
            new Keyframe(0f, 0f)
        });

        public static AudioSource CreateSource(Transform transform, Vector3 position, float minDistance = 1f, float maxDistance = 500f, AudioMixerGroup mixerGroup = null)
        {
            var sourceObj = new GameObject("CCL_AudioSource");
            sourceObj.transform.SetParent(transform);
            sourceObj.transform.position = position;

            var source = sourceObj.AddComponent<AudioSource>();
            source.Stop();
            source.loop = false;
            source.playOnAwake = false;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, resetCurve);
            source.SetCustomCurve(AudioSourceCurveType.SpatialBlend, resetCurve);
            source.SetCustomCurve(AudioSourceCurveType.Spread, resetCurve);
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.spread = 0f;
            source.pitch = 1f;
            source.volume = 1f;
            source.spatialBlend = 1f;
            source.dopplerLevel = 0f;
            source.ignoreListenerPause = false;
            source.outputAudioMixerGroup = mixerGroup;

            return source;
        }
    }
}
