using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public class IndicatorPortReaderProxy : MonoBehaviour, IHasPortIdFields, IHasFuseIdFields
    {
        [PortId(null, null, false)]
        public string portId;

        [PortId(null, null, false)]
        [Header("Optional")]
        public string indicatorRangeScalerPortId;

        [FuseId]
        public string fuseId;

        [Header("Value modifiers")]
        public float valueMultiplier = 1f;

        public float valueOffset;

        public bool useAbsoluteValue;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(portId), portId),
            new PortIdField(this, nameof(indicatorRangeScalerPortId), indicatorRangeScalerPortId),
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(fuseId), fuseId),
        };
    }
}
