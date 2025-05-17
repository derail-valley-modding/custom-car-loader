using UnityEngine;

namespace CCL.Types.Proxies.Customization
{
    public class DrillingDisablerProxy : MonoBehaviour
    {
        [Tooltip("Whether or not to allow drilling for all child colliders. Overrides parent.")]
        public bool allowDrilling;
    }
}
