using CCL.Types;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Simulation.Electric;
using UnityEngine;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class MultipleUnitValidator : LiveryValidator
    {
        public override string TestName => "Multiple Unit";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            if (!livery.HasMUCable)
            {
                if (ComponentUtil.HasComponentInChildren<SlugsPowerProviderModuleProxy>(livery.prefab!))
                {
                    return Fail($"{livery.id} - {nameof(SlugsPowerProviderModuleProxy)} found but livery has no MU cable", livery.prefab);
                }

                return Skip();
            }

            var result = Pass();
            var prefab = livery.prefab!;

            CheckComp<BaseControlsOverriderProxy>();
            CheckComp<SimConnectionsDefinitionProxy>();
            CheckComp<DamageControllerProxy>();

            return result;
            
            void CheckComp<T>() where T : Component
            {
                if (!ComponentUtil.HasComponentInChildren<T>(prefab))
                {
                    result.Fail($"Livery '{livery.id}' has MU cable but lacks {typeof(T).Name}", livery);
                }
            }
        }
    }
}
