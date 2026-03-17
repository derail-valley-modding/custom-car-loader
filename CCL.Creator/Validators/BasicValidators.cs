using CCL.Types;
using CCL.Types.Proxies.Customization;
using System.Linq;
using UnityEngine;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(CarSettingsValidator))]
    internal class LiverySettingsValidator : LiveryValidator
    {
        public override string TestName => "Livery Setup";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            if (livery.prefab == null)
            {
                return CriticalFail("Livery must have a prefab assigned!", livery, nameof(livery.prefab));
            }

            if (string.IsNullOrWhiteSpace(livery.id))
            {
                return Fail("Livery has no ID set", livery, nameof(livery.id));
            }

            var result = Pass();

            if (livery.icon == null)
            {
                result.Warning($"Livery '{livery.id}' has no icon", livery, nameof(livery.icon));
            }

            if (livery.LocoSpawnGroups.TryFind(x => x.IsDisallowedSpawn(), out var disallowed))
            {
                result.Fail($"Livery '{livery.id}' is set to spawn on a disallowed track ({disallowed.Track})",
                    livery, nameof(livery.LocoSpawnGroups));
            }
            else if (livery.LocoSpawnGroups.TryFind(x => x.IsDE2ExclusiveSpawn(), out var exlusive))
            {
                result.Warning($"Livery '{livery.id}' is set to spawn on a DE2 exclusive track ({exlusive.Track}), make sure this is intended",
                    livery, nameof(livery.LocoSpawnGroups));
            }

            if (!ComponentUtil.HasComponent<CustomizationPlacementMeshesProxy>(livery.prefab))
            {
                result.Warning($"Livery prefab does not have {nameof(CustomizationPlacementMeshesProxy)}, " +
                    "gadgets will not be able to be attached to this prefab", livery.prefab);
            }

            if (livery.TrainsetLiveries.Any(string.IsNullOrWhiteSpace))
            {
                result.Warning("Livery trainset has blank entries", livery, nameof(livery.TrainsetLiveries));
            }
            else if (livery.TrainsetLiveries.ContainsDuplicates())
            {
                result.Warning("Livery trainset has duplicates", livery, nameof(livery.TrainsetLiveries));
            }

            if (livery.LocoSpawnGroups.Length > 0 && livery.UnlockableAsWorkTrain)
            {
                result.Warning("Livery can spawn in the world but is also set as a work train, this can result in unintended behaviour", livery);
            }

            return result;
        }
    }

    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class RootTransformValidator : LiveryValidator
    {
        public override string TestName => "Root Transform";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            if (livery.prefab!.transform.position != Vector3.zero)
            {
                return Fail($"{livery.id} - Not at (0,0,0)");
            }
            if (livery.prefab.transform.eulerAngles != Vector3.zero)
            {
                return Fail($"{livery.id} - Non-zero rotation");
            }
            if (livery.prefab.transform.localScale != Vector3.one)
            {
                return Fail($"{livery.id} - Scale is not 1");
            }
            return Pass();
        }
    }
}
