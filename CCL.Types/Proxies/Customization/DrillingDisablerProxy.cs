using UnityEngine;

namespace CCL.Types.Proxies.Customization
{
    [AddComponentMenu("CCL/Proxies/Customization/Drilling Disabler Proxy")]
    public class DrillingDisablerProxy : MonoBehaviour, ISelfValidation
    {
        [Tooltip("Whether or not to allow drilling for all child colliders. Overrides parent.")]
        public bool allowDrilling;

        public SelfValidationResult Validate(out string message)
        {
            var colliders = GetComponentsInChildren<Collider>(true);

            if (colliders.Length == 0)
            {
                message = "no colliders";
                return SelfValidationResult.Warning;
            }

            foreach (var collider in colliders)
            {
                if (collider is MeshCollider) continue;

                if (!collider.isTrigger)
                {
                    message = $"{collider.GetType().Name} is not set as trigger";
                    return SelfValidationResult.Warning;
                }
            }

            return this.Pass(out message);
        }
    }
}
