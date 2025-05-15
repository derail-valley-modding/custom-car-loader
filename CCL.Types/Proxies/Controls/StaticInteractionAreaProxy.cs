using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
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