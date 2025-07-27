using CCL.Types.Proxies.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Creator.Utility
{
    [AddComponentMenu("CCL Editor/Audio Tester")]
    internal class AudioTester : MonoBehaviour
    {
        public LayeredAudioProxy Audio = null!;
        [Range(0f, 1f)]
        public float InputValue = 0.0f;
        public bool SelfChange = false;
        public LayeredAudioPortReaderProxy.UpdateType UpdateType;

        private Dictionary<AudioLayerProxy, float> _startPitch = new Dictionary<AudioLayerProxy, float>();

        private void Start()
        {
            _startPitch.Clear();

            foreach (var layer in Audio.layers)
            {
                layer.source.enabled = true;
                layer.source.loop = true;
                layer.source.Play();
                _startPitch.Add(layer, layer.source.pitch);
            }
        }

        private void Update()
        {
            if (SelfChange)
            {
                InputValue = 1.1f - Mathf.Abs(((Time.timeSinceLevelLoad / 4.0f) % 2.4f) - 1.2f);
            }

            InputValue = Mathf.Clamp01(InputValue);

            switch (UpdateType)
            {
                case LayeredAudioPortReaderProxy.UpdateType.SET_VOLUME_AND_PITCH:
                    SetVolume(InputValue);
                    SetPitch(InputValue);
                    break;
                case LayeredAudioPortReaderProxy.UpdateType.SET_VOLUME:
                    SetVolume(InputValue);
                    SetPitch(1);
                    break;
                case LayeredAudioPortReaderProxy.UpdateType.SET_PITCH:
                    SetVolume(1);
                    SetPitch(InputValue);
                    break;
                default:
                    break;
            }
        }

        private void SetVolume(float value)
        {
            foreach(var layer in Audio.layers)
            {
                layer.source.volume = layer.volumeCurve.Evaluate(value);
            }
        }

        private void SetPitch(float value)
        {
            foreach (var layer in Audio.layers)
            {
                float pitch = _startPitch[layer];

                layer.source.pitch = layer.usePitchCurve ?
                    pitch * layer.pitchCurve.Evaluate(value) :
                    pitch * Mathf.Lerp(Audio.minPitch, Audio.maxPitch, value);
            }
        }
    }
}
