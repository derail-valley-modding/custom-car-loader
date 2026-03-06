using CCL.Types;
using CCL.Types.Proxies.Simulation.Steam;
using UnityEngine;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class ComponentValidator : LiveryValidator
    {
        public override string TestName => "Components";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            int count = 0;
            var result = Pass();

            foreach (var prefab in livery.AllPrefabs)
            {
                var components = prefab.GetComponentsInChildren<ISelfValidation>();

                count += components.Length;

                foreach (var comp in components)
                {
                    if (!(comp is Component self)) continue;

                    switch (comp.Validate(out var message))
                    {
                        case SelfValidationResult.Info:
                            result.AddInformation($"{livery.id} - {message}");
                            break;
                        case SelfValidationResult.Warning:
                            result.Warning(AddCompToMessage(self, message), self);
                            break;
                        case SelfValidationResult.Fail:
                            result.Fail(AddCompToMessage(self, message), self);
                            break;
                        case SelfValidationResult.Critical:
                            result.CriticalFail(AddCompToMessage(self, message), self);
                            return result;
                        default:
                            continue;
                    }
                }
            }

            count += TestLubricatorRatchet(livery, result);

            return count == 0 ? Skip() : result;

            static string AddCompToMessage(Component comp, string message)
            {
                return $"{comp.name}/{comp.GetType().Name}: {message}";
            }
        }

        private int TestLubricatorRatchet(CustomCarVariant livery, ValidationResult result)
        {
            var comps = livery.AllPrefabs.GetComponentsInChildren<LubricatorRatchetProxy>();

            if (comps.Count > 0 && !ComponentUtil.HasComponentInChildren<LubricatorRatchetDriverProxy>(livery.prefab!))
            {
                result.Fail($"Could not find any {nameof(LubricatorRatchetDriverProxy)}", livery.prefab);
            }

            return comps.Count;
        }
    }
}
