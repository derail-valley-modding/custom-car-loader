using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Static Interaction Area Proxy")]
    public class StaticInteractionAreaProxy : MonoBehaviour
    {
        public void OnValidate()
        {
            foreach (var item in GetComponentsInChildren<Collider>(true))
            {
                item.isTrigger = true;
            }
        }
    }
}