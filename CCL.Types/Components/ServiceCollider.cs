using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Service Collider")]
    [RequireComponent(typeof(Collider))]
    public class ServiceCollider : MonoBehaviour
    {
        private void Start()
        {
            tag = CarPartNames.Tags.MAIN_TRIGGER_COLLIDER;
            Destroy(this);
        }
    }
}
