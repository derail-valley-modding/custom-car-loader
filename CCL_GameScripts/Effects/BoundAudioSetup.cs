using CCL_GameScripts.Attributes;
using System.Collections;
using UnityEngine;

namespace CCL_GameScripts.Effects
{
    public abstract class BoundAudioSetup : ComponentInitSpec
    {
        [ProxyField]
        public Transform PlaySoundAt;
        [ProxyField]
        public AudioClip Clip;
        [ProxyField]
        public DVAudioMixerGroup MixerGroup = DVAudioMixerGroup.External;

        public ConfigurableBinding Pitch;
        public ConfigurableBinding Volume;

        [HideInInspector]
        [ProxyField]
        public string BindingData;

        public abstract void OnValidate();
    }
}