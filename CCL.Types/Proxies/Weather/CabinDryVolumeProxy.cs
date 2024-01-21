using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    [RequireComponent(typeof(WetDecalProxy))]
    public class CabinDryVolumeProxy : MonoBehaviour
    {
        public WetDecalProxy[] subVolumes = new WetDecalProxy[0];

        public float distance;

        public float edgeFadeOffInside = 0.05f;

        public float edgeFadeOffOutside = 1f;

        [RenderMethodButtons]
        [MethodButton("CCL.Types.Proxies.Weather.CabinDryVolumeProxy:AutoAssignSubVolumes", "Auto assign children as subvolumes")]
        public bool buttonRender;

        private static void AutoAssignSubVolumes(CabinDryVolumeProxy proxy)
        {
            proxy.subVolumes = proxy.GetComponentsInChildren<WetDecalProxy>(true).Where(c => c.transform != proxy.transform).ToArray();
        }
    }
}
