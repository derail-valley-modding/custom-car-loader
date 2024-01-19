using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class CabLightsControllerProxy : PoweredControlHandlerBase
    {
        public Material lightsLit;

        public Material lightsUnlit;

        [Tooltip("Light components will be enabled/disabled based on threshold")]
        public GameObject[] lights;
        [Tooltip("Renderers will have material changed between lightsLit/lightsUnlit")]
        public Renderer[] lightRenderers;

        public float lightsOnControlThreshold = 0.5f;

        public float damagedThresholdPercentage = 0.8f;
    }
}
