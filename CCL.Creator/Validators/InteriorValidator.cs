using CCL.Types;
using CCL.Types.Proxies.Weather;
using UnityEngine;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class InteriorValidator : LiveryValidator
    {
        public override string TestName => "Interior";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            var result = Pass();

            if (livery.interiorPrefab != null)
            {
                if (livery.interiorPrefab.transform.localPosition != Vector3.zero)
                {
                    result.Warning($"'{livery.id}' - Interior is not centered at {Vector3.zero}", livery.interiorPrefab);
                }

                var openables = livery.interiorPrefab.GetComponentsInChildren<OpenableControlProxy>();

                if (openables.Length > 0)
                {
                    result.Warning($"'{livery.id}' - Openables are not supported in the interior prefab", livery.interiorPrefab);
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
                            result.Warning($"'{livery.id}' - Renderer {item.name} in {CarPartNames.INTERIOR_LOD} has shadow casting turned on, " +
                                $"it should be set to off", item);
                        }
                    }
                }
                else if (livery.interiorPrefab != null)
                {
                    result.Warning($"{livery.id} - Interior prefab exists but {CarPartNames.INTERIOR_LOD} does not", livery.interiorPrefab);
                }
            }

            if (livery.parentType != null && livery.parentType.HUDLayout != null && livery.interiorPrefab == null)
            {
                result.Fail($"{livery.id} - HUD layout needs an interior prefab to exist");
            }

            return result;
        }
    }
}
