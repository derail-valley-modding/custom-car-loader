using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Static Interaction Area Proxy")]
    public class StaticInteractionAreaProxy : MonoBehaviour, ISelfValidation
    {
        public void OnValidate()
        {
            foreach (var item in GetComponentsInChildren<Collider>(true))
            {
                item.isTrigger = true;
            }
        }

        public SelfValidationResult Validate(out string message)
        {
            if (GetComponentInParent<ControlSpecProxy>())
            {
                message = $"static area under a moving control defeats its purpose";
                return SelfValidationResult.Warning;
            }

            return this.Pass(out message);
        }
    }
}