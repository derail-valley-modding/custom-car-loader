using UnityEngine;

namespace CCL.Types.Proxies.VFX
{
    [AddComponentMenu("CCL/Proxies/VFX/Item Light Proxy")]
    public class ItemLightProxy : MonoBehaviour, ISelfValidation
    {
        public Light light = null!;

        public SelfValidationResult Validate(out string message, out string? highlight)
        {
            if (light == null)
            {
                return this.FailForNull(nameof(light), out message, out highlight);
            }
            if (GetComponent<LightShadowQualityProxy>())
            {
                message = $"{nameof(ItemLightProxy)} and {nameof(LightShadowQualityProxy)} are not compatible with each other! Remove one!";
                highlight = null;
                return SelfValidationResult.Fail;
            }

            return this.Pass(out message, out highlight);
        }
    }
}
