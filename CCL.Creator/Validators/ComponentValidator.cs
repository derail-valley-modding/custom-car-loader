using CCL.Types;
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

            return count == 0 ? Skip() : result;

            static string AddCompToMessage(Component comp, string message)
            {
                return $"{comp.name}/{comp.GetType().Name}: {message}";
            }
        }
    }
}
