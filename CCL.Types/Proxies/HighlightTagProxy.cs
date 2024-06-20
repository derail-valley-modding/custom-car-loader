using UnityEngine;

namespace CCL.Types.Proxies
{
    public class HighlightTagProxy : MonoBehaviour
    {
        [Tooltip("Optional, leave empty to use this gameObject")]
        public GameObject targetObject;
        public float overrideDistance;
    }
}
