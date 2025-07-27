using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    [AddComponentMenu("CCL/Proxies/Weather/Cabin Dry Volume Proxy")]
    [RequireComponent(typeof(WetDecalProxy))]
    public class CabinDryVolumeProxy : MonoBehaviour
    {
        public WetDecalProxy[] subVolumes = new WetDecalProxy[0];
        public float distance;
        public float edgeFadeOffInside = 0.05f;
        public float edgeFadeOffOutside = 1f;

        [RenderMethodButtons, SerializeField]
        [MethodButton(nameof(AutoAssignSubVolumes), "Auto assign children as subvolumes")]
        private bool _buttons;

        private void AutoAssignSubVolumes()
        {
            subVolumes = GetComponentsInChildren<WetDecalProxy>(true).Where(c => c.transform != transform).ToArray();
        }
    }
}
