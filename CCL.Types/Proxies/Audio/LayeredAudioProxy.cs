﻿using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Audio
{
    [AddComponentMenu("CCL/Proxies/Audio/Layered Audio Proxy")]
    public class LayeredAudioProxy : MonoBehaviour
    {
        public float masterVolume = 1.0f;
        public Type type;
        public bool linearPitchLerp = true;
        public float minPitch = 1f;
        public float maxPitch = 1f;

        //public AudioMixerGroup audioMixerGroup;
        public DVAudioMixerGroup audioMixGroup = DVAudioMixerGroup.Engine;

        public bool randomizeStartTime = true;
        public List<AudioLayerProxy> layers = new List<AudioLayerProxy>();

        [RenderMethodButtons, SerializeField]
        [MethodButton(nameof(AddLayer), "Add New Layer")]
        private bool _renderButtons;

        public void AddLayer()
        {
            var holder = new GameObject("newLayer");
            holder.transform.SetParent(transform, false);

            var layer = holder.AddComponent<AudioLayerProxy>();
            var source = holder.AddComponent<AudioSource>();
            layer.source = source;

            source.playOnAwake = false;
            source.minDistance = 3;
            source.maxDistance = 400;
            source.volume = 1;
            source.spatialBlend = 1; // full 3d
            source.dopplerLevel = 0;
            source.ignoreListenerPause = false;

            holder.AddComponent<DopplerProxy>();

            layers ??= new List<AudioLayerProxy>();
            layers.Add(layer);
        }

        public enum Type
        {
            Continuous,
            OneTime
        }
    }

    [AddComponentMenu("CCL/Proxies/Audio/Audio Layer Proxy")]
    public class AudioLayerProxy : MonoBehaviour
    {
        public string name;
        public AnimationCurve volumeCurve = null!;
        public bool usePitchCurve;
        public AnimationCurve pitchCurve = null!;
        public float inertia;
        public bool inertialPitch;

        [Tooltip("For Continuous: AudioSource to play and set values to.")]
        // \nFor One Time: This source will be used as a template for automatically created individual sources
        public AudioSource source = null!;

        // Below used only for testing and the built in track joints audio

        //[Tooltip("If assigned, these will be used randomly on the source. Used only for one time effects (for now)")]
        //public AudioClip[] clips;

        //[Tooltip("Pitch variation for each clip play")]
        //public Vector2 pitchRange = new Vector2(1f, 1f);
    }
}
