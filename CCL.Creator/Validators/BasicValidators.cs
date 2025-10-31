using CCL.Types;
using CCL.Types.Proxies.Customization;
using System.Linq;
using UnityEngine;

namespace CCL.Creator.Validators
{
    internal class LiverySettingsValidator : LiveryValidator
    {
        public override string TestName => "Car Setup";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            if (livery.prefab == null)
            {
                return CriticalFail("Livery must have a prefab assigned!");
            }

            if (string.IsNullOrWhiteSpace(livery.id))
            {
                return Fail("Livery has no ID set");
            }

            var result = Pass();

            if (livery.prefab.GetComponent<CustomizationPlacementMeshesProxy>() == null)
            {
                result.Warning("Livery prefab does not have CustomizationPlacementMeshesProxy, " +
                    "gadgets will not be able to be attached to this prefab", livery.prefab);
            }

            if (livery.TrainsetLiveries.Any(string.IsNullOrWhiteSpace))
            {
                result.Warning("Livery trainset has blank entries");
            }
            else if (livery.TrainsetLiveries.ContainsDuplicates())
            {
                result.Warning("Livery trainset has duplicates");
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
                        result.Warning("Missing renderers on LOD");
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
                    result.Warning($"Interior is not centered at {Vector3.zero}");
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
                            result.Warning($"Renderer {item.name} in {CarPartNames.INTERIOR_LOD} has shadow casting turned on, it should be set to off");
                        }
                    }
                }
                else if (livery.interiorPrefab != null)
                {
                    result.Warning($"{livery.id} - Interior prefab exists but {CarPartNames.INTERIOR_LOD} does not");
                }
            }

            return result;
        }
    }
}
