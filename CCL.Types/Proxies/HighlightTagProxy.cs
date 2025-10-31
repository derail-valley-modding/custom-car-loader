using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Highlight Tag Proxy"), DisallowMultipleComponent]
    public class HighlightTagProxy : MonoBehaviour
    {
        [Tooltip("Optional, leave empty to use this gameObject")]
        public GameObject targetObject = null!;
        public List<Renderer> renderers = new List<Renderer>();
        public float overrideDistance;
    }
}
