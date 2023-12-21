
using CCL.Types.Proxies.Ports;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public class IndicatorPortReaderProxy : MonoBehaviour
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
    }
}
