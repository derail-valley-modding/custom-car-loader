using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class InteriorControlsManagerProxy : MonoBehaviour
    {
        public bool electricsFuseAffectsIndicators = true;
        public List<DVControlType> reverseDirectionList = new List<DVControlType>();
    }
}
