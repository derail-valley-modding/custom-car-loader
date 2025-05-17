using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    public class WiperProxy : MonoBehaviour
    {
        public List<WindowProxy> windows = new List<WindowProxy>();
        public Transform start = null!;
        public Transform end = null!;
    }
}
