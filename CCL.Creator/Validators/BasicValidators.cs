using CCL.Types;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Customization;
using CCL.Types.Proxies.Ports;
using System.Linq;
using UnityEngine;

namespace CCL.Creator.Validators
{
    internal class LiverySettingsValidator : LiveryValidator
    {
        public override string TestName => "Livery Setup";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            if (livery.prefab == null)
            {
                return CriticalFail("Livery must have a prefab assigned!", livery);
            }

            if (string.IsNullOrWhiteSpace(livery.id))
            {
                return Fail("Livery has no ID set", livery);
            }

            var result = Pass();

            if (livery.icon == null)
            {
                result.Warning($"Livery '{livery.id}' has no icon", livery);
            }

            if (livery.LocoSpawnGroups.TryFind(x => x.IsDisallowedSpawn(), out var disallowed))
            {
                result.Fail($"Livery '{livery.id}' is set to spawn on a disallowed track ({disallowed.Track})", livery);
            }
            else if (livery.LocoSpawnGroups.TryFind(x => x.IsDE2ExclusiveSpawn(), out var exlusive))
            {
                result.Warning($"Livery '{livery.id}' is set to spawn on a DE2 exclusive track ({exlusive.Track}), make sure this is intended", livery);
            }

            if (!ComponentUtil.HasComponent<CustomizationPlacementMeshesProxy>(livery.prefab, false))
            {
                result.Warning($"Livery prefab does not have {nameof(CustomizationPlacementMeshesProxy)}, " +
                    "gadgets will not be able to be attached to this prefab", livery.prefab);
            }

            if (livery.TrainsetLiveries.Any(string.IsNullOrWhiteSpace))
            {
                result.Warning("Livery trainset has blank entries", livery);
            }
            else if (livery.TrainsetLiveries.ContainsDuplicates())
            {
                result.Warning("Livery trainset has duplicates", livery);
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

    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class LODGroupValidator : LiveryValidator
    {
        public override string TestName => "LOD Group";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            var result = Pass();
            var lodGroup = livery.prefab!.GetComponent<LODGroup>();
            if (lodGroup)
            {
                foreach (var lod in lodGroup.GetLODs())
                {
                    if (lod.renderers.Length == 0)
                    {
                        result.Warning("Missing renderers on LOD", lodGroup);
                    }
                }
            }
            return result;
        }
    }

    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class InteriorTransformValidator : LiveryValidator
    {
        public override string TestName => "Interior";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            var result = Pass();

            if (livery.interiorPrefab != null)
            {
                if (livery.interiorPrefab.transform.localPosition != Vector3.zero)
                {
                    result.Warning($"Interior is not centered at {Vector3.zero}", livery.interiorPrefab);
                }
            }

            if (livery.prefab != null)
            {
                if (livery.prefab.transform.TryFind(CarPartNames.INTERIOR_LOD, out var interiorLOD))
                {
                    foreach (var item in interiorLOD.GetComponentsInChildren<Renderer>(true))
                    {
                        if (item.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off)
                        {
                            result.Warning($"Renderer {item.name} in {CarPartNames.INTERIOR_LOD} has shadow casting turned on, it should be set to off", item);
                        }
                    }
                }
                else if (livery.interiorPrefab != null)
                {
                    result.Warning($"{livery.id} - Interior prefab exists but {CarPartNames.INTERIOR_LOD} does not", livery.interiorPrefab);
                }
            }

            return result;
        }
    }

    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class MultipleUnitValidator : LiveryValidator
    {
        public override string TestName => "Multiple Unit";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            if (!livery.HasMUCable) return Skip();

            var result = Pass();
            var prefab = livery.prefab!;

            CheckComp<BaseControlsOverriderProxy>();
            CheckComp<SimConnectionsDefinitionProxy>();
            CheckComp<DamageControllerProxy>();

            return result;
            
            void CheckComp<T>() where T : Component
            {
                if (!ComponentUtil.HasComponent<T>(prefab))
                {
                    result.Fail($"Livery has MU cable but lacks {typeof(T).Name}", livery);
                }
            }
        }
    }
}
