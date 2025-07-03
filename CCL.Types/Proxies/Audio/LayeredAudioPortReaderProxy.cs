using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Audio
{
    [AddComponentMenu("CCL/Proxies/Audio/Layered Audio Port Reader Proxy")]
    public class LayeredAudioPortReaderProxy : MonoBehaviour, IHasPortIdFields
    {
        public UpdateType updateType;

        [PortId]
        public string portId = string.Empty;
        public float valueMultiplier = 1f;
        public float valueOffset;
        public bool absoluteInputValue;
        public bool absoluteResultValue;

        [Header("Optional Custom Mapping")]
        public bool useValueMapper;
        public float inMapMin;
        public float inMapMax;
        public float outMapMin;
        public float outMapMax;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(portId), portId),
        };

        public enum UpdateType
        {
            SET_VOLUME_AND_PITCH,
            SET_VOLUME,
            SET_PITCH,
            SET_MASTER_VOLUME
        }
    }
}
