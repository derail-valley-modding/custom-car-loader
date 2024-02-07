using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Audio
{
    public class AudioClipPortReaderProxy : MonoBehaviour, IHasPortIdFields
    {
        public AudioClip[] clips;
        public Transform positionAnchor;

        public float volume = 1f;
        public float pitch = 1f;

        public float spread;
        public float minDistance = 1f;
        public float maxDistance = 500f;

        //public AudioMixerGroup mixerGroup;

        public bool isParented = true;

        public PlayClipType playType;
        public float playAudioThreshold;

        [PortId]
        public string portId;

        public float valueMultiplier = 1f;
        public float valueOffset;

        public bool absoluteInputValue;
        public bool absoluteResultValue;

        [Header("Optional Mapping")]
        public bool useValueMapper;

        public float inMapMin;
        public float inMapMax;
        public float outMapMin;
        public float outMapMax;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(portId), portId),
        };

        public enum PlayClipType
        {
            PLAY_ON_ABOVE_THRESHOLD,
            PLAY_ON_BELOW_THRESHOLD,
            PLAY_ON_EQUAL_TO_THRESHOLD
        }
    }
}
