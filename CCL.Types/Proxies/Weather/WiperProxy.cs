using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    [AddComponentMenu("CCL/Proxies/Weather/Wiper Proxy")]
    public class WiperProxy : MonoBehaviour
    {
        public List<WindowProxy> windows = new List<WindowProxy>();
        public Transform start = null!;
        public Transform end = null!;
    }
}
