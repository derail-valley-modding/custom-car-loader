using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    [AddComponentMenu("CCL/Proxies/Indicators/Indicator Emission Proxy")]
    public class IndicatorEmissionProxy : IndicatorProxy, ISelfValidation
    {
        [Header("Behaviour")]
        [Tooltip("If enabled, the lamp can only be fully on or fully off.")]
        public bool binary = true;
        [Tooltip("The lamp will only be lit if the value if higher than this.")]
        public float valueThreshold = 0.5f;
        [Tooltip("How many seconds does it take for lamp to light. Default: 0.05")]
        public float lag = 0.05f;

        [Header("Emission")]
        public Color emissionColor = Color.white;
        public Light emissionLight = null!;
        public float lightIntensity = 1f;
        [Tooltip("(Optional) The renderers to apply the emission color to. It'll use the first child renderer if unset.")]
        public Renderer[] renderers = new Renderer[0];

        [Header("Glare")]
        public Color glareColor = Color.white;
        public Renderer glareRenderer = null!;

        public SelfValidationResult Validate(out string message)
        {
            if (renderers.Length == 0 && GetComponentsInChildren<Renderer>().Length == 0)
            {
                message = "no renderers assigned.";
                return SelfValidationResult.Fail;
            }

            foreach (var renderer in renderers)
            {
                if (renderer == null)
                {
                    message = "renderer is null.";
                    return SelfValidationResult.Fail;
                }
            }

            return this.Pass(out message);
        }
    }
}
