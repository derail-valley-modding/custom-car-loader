using CCL_GameScripts;
using CCL_GameScripts.CabControls;
using CCL_GameScripts.Effects;
using DVCustomCarLoader.LocoComponents;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DVCustomCarLoader.Effects
{
    public abstract class BoundAudioSource : MonoBehaviour, ILocoEventAcceptor, ICabControlAcceptor
    {
        public AudioSource Source;

        public Transform PlaySoundAt;
        public AudioClip Clip;
        public DVAudioMixerGroup MixerGroup = DVAudioMixerGroup.External;

        protected SimEventType[] _eventBindings;
        public SimEventType[] EventTypes => _eventBindings;

        public string BindingData;
        protected List<ConfigurableBindingBase> _bindings = new List<ConfigurableBindingBase>();

        protected virtual void CreateBindings(JSONObject bindings)
        {
            var pitch = ConfigurableBindingBase.FromJSON<ConfigurableBinding>(bindings["pitch"]);
            var volume = ConfigurableBindingBase.FromJSON<ConfigurableBinding>(bindings["volume"]);

            pitch.ApplyFunc = SetPitch;
            volume.ApplyFunc = SetVolume;

            _bindings.Add(pitch);
            _bindings.Add(volume);
        }

        protected JSONObject Initialize()
        {
            var bindings = new JSONObject(BindingData);
            CreateBindings(bindings);

            _eventBindings = _bindings
                .Where(b => b.OutputBinding != SimEventType.None)
                .Select(b => b.OutputBinding)
                .ToArray();

            Vector3 position = PlaySoundAt ? PlaySoundAt.position : transform.position;
            var mixerGroup = AudioUtils.GetMixerGroup(MixerGroup);
            Source = AudioUtils.CreateSource(transform, position, mixerGroup: mixerGroup);
            Source.clip = Clip;

            foreach (var binding in _bindings)
            {
                binding.Normalize();
                binding.Reset();
            }

            return bindings;
        }

        protected void SetPitch(float pitch) => Source.pitch = pitch;
        protected void SetVolume(float volume) => Source.volume = volume;

        public void HandleEvent(LocoEventInfo eventInfo)
        {
            if (eventInfo.NewValue is float newVal)
            {
                foreach (var binding in _bindings)
                {
                    if (binding.OutputMatches(eventInfo.EventType))
                    {
                        binding.Apply(newVal);
                    }
                }
            }
        }

        public bool AcceptsControlOfType(CabInputType inputType)
        {
            return _bindings.Any(b => b.ControlMatches(inputType));
        }

        public void RegisterControl(CabInputRelay controlRelay)
        {
            foreach (var binding in _bindings)
            {
                if (binding.ControlMatches(controlRelay.Binding))
                {
                    controlRelay.AddListener(binding.HandleEvent);
                }
            }
        }
    }
}