using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Interior Non Standard Layer Proxy")]
    public class InteriorNonStandardLayerProxy : MonoBehaviour
    {
        public DVLayer Layer = DVLayer.Train_Walkable;
        public bool includeChildren;

        public static InteriorNonStandardLayerProxy AttachToObject(GameObject go, DVLayer layer, bool includeChildren = false)
        {
            var proxy = go.AddComponent<InteriorNonStandardLayerProxy>();
            proxy.Layer = layer;
            proxy.includeChildren = includeChildren;
            return proxy;
        }
    }
}
