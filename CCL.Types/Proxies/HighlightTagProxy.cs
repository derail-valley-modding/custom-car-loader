using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Highlight Tag Proxy")]
    public class HighlightTagProxy : MonoBehaviour
    {
        [Tooltip("Optional, leave empty to use this gameObject")]
        public GameObject targetObject = null!;
        public float overrideDistance;
    }
}
