using UnityEngine;

namespace CCL.Types.Proxies.Interaction
{
    [AddComponentMenu("CCL/Proxies/Interaction/Item Use Redirect Proxy")]
    public class ItemUseRedirectProxy : MonoBehaviour, ISelfValidation
    {
        public ItemUseTargetProxy target = null!;

        public SelfValidationResult Validate(out string message, out string? highlight)
        {
            if (target == null)
            {
                return this.FailForNull(nameof(target), out message, out highlight);
            }

            return this.Pass(out message, out highlight);
        }
    }
}
