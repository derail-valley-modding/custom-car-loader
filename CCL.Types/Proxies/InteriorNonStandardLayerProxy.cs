using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Interior Non Standard Layer Proxy")]
    public class InteriorNonStandardLayerProxy : MonoBehaviour
    {
        public DVLayer Layer = DVLayer.Train_Walkable;
        public bool includeChildren;
    }
}
