using UnityEngine;

namespace CCL.Types.Proxies.Interaction
{
    [AddComponentMenu("CCL/Proxies/Interaction/Item Use Target Proxy")]
    public class ItemUseTargetProxy : MonoBehaviour
    {
        public Collider[] targetColliders = new Collider[0];
    }
}
