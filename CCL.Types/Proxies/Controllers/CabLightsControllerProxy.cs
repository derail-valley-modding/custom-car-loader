using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class CabLightsControllerProxy : PoweredControlHandlerBase
    {
        public Material lightsLit = null!;
        public Material lightsUnlit = null!;
        [Tooltip("Light components will be enabled/disabled based on threshold")]
        public GameObject[] lights = new GameObject[0];
        [Tooltip("Renderers will have material changed between lightsLit/lightsUnlit")]
        public Renderer[] lightRenderers = new Renderer[0];
        public float lightsOnControlThreshold = 0.5f;
        public float damagedThresholdPercentage = 0.8f;
    }
}
