using UnityEngine;

namespace CCL.Types.Proxies.Headlights
{
    [AddComponentMenu("CCL/Proxies/Headlights/Headlight Beam Controller Proxy")]
    public class HeadlightBeamControllerProxy : VolumetricBeamControllerBaseProxy
    {
        public HeadlightsMainControllerProxy headlightsMainController = null!;
        public float intensityMultiplier = 1f;
    }
}
