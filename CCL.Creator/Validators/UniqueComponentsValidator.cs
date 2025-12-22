using CCL.Types;
using CCL.Types.Proxies;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Headlights;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.VFX;
using CCL.Types.Proxies.Weather;
using CCL.Types.Proxies.Wheels;
using UnityEngine;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class UniqueComponentsValidator : LiveryValidator
    {
        public override string TestName => "Unique Components";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            ValidationResult? result = null;
            var prefab = livery.prefab!;

            NoMoreThanOne<PoweredWheelsManagerProxy>(prefab);
            NoMoreThanOne<BaseControlsOverriderProxy>(prefab);
            NoMoreThanOne<BasePortsOverriderProxy>(prefab);
            NoMoreThanOne<HeadlightsMainControllerProxy>(prefab);
            NoMoreThanOne<CabLightsControllerProxy>(prefab);
            NoMoreThanOne<WiperControllerProxy>(prefab);
            NoMoreThanOne<ParticlesPortReadersControllerProxy>(prefab);
            NoMoreThanOne<TractionPortFeedersProxy>(prefab);
            NoMoreThanOne<CompressorSimControllerProxy>(prefab);
            NoMoreThanOne<CoalPileSimControllerProxy>(prefab);
            NoMoreThanOne<FireboxSimControllerProxy>(prefab);
            NoMoreThanOne<RemoteControllerModuleProxy>(prefab);

            return result ?? Skip();

            void NoMoreThanOne<T>(GameObject prefab)
            {
                if (prefab.GetComponentsInChildren<T>().Length > 1)
                {
                    result ??= Pass();

                    result.Fail($"Cannot have more than 1 {typeof(T).Name} component in the prefab");
                }
            }
        }
    }
}
