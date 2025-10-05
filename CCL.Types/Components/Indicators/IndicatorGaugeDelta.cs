using CCL.Types.Proxies.Indicators;
using UnityEngine;

namespace CCL.Types.Components.Indicators
{
    [AddComponentMenu("CCL/Components/Indicators/Indicator Gauge Delta")]
    public class IndicatorGaugeDelta : IndicatorGaugeProxy
    {
        [Header("Delta properties")]
        public float updateThreshold = 0.001f;
        public float maxDelta = 20.0f;
        [Tooltip("If true, Max Delta represents the time it takes to reach the new value, " +
            "instead of the maximum rotation per second")]
        public bool deltaIsTime = false;
    }
}
