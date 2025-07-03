using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Interior Controls Manager Proxy")]
    public class InteriorControlsManagerProxy : MonoBehaviour
    {
        public bool electricsFuseAffectsIndicators = true;
        public List<DVControlType> reverseDirectionList = new List<DVControlType>();
    }
}
