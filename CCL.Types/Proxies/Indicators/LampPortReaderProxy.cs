using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public class LampPortReaderProxy : MonoBehaviour, IHasPortIdFields, IHasFuseIdFields
    {
        [PortId]
        public string portId;

        [FuseId]
        [Header("Optional")]
        public string fuseId;

        [Header("Behaviour setup")]
        public float offRangeMin;

        public float offRangeMax;

        [Space]
        public bool onRangeUsed;

        public float onRangeMin;

        public float onRangeMax;

        [Space]
        public bool blinkRangeUsed;

        public float blinkRangeMin;

        public float blinkRangeMax;

        [Space]
        public bool playAudioOnValueDrop;

        public bool playAudioOnValueRaise;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(portId), portId),
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(fuseId), fuseId),
        };
    }
}
