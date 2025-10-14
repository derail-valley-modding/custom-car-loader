using UnityEngine;

namespace CCL.Types.Proxies.Interaction
{
    [AddComponentMenu("CCL/Proxies/Interaction/Item Use Redirect Proxy")]
    public class ItemUseRedirectProxy : MonoBehaviour
    {
        public ItemUseTargetProxy target = null!;
    }
}
