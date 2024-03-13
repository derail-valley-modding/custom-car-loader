using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public class IndicatorEmissionProxy : IndicatorProxy
    {
        [Tooltip("If set, it's either on or off")]
        public bool binary = true;

        [Tooltip("if the value is higher than this, lamp will be lit. Only used if binary is on")]
        public float valueThreshold = 0.5f;

        [Tooltip("How many seconds does it take for lamp to light. Default: 0.05")]
        public float lag = 0.05f;

        [Tooltip("Optional, if not set, it will use child renderer")]
        public Renderer[] renderers;

        public Color emissionColor = Color.white;
        public Light emissionLight;
    }
}
