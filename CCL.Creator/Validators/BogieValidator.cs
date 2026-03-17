using CCL.Types;
using UnityEngine;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class BogieValidator : LiveryValidator
    {
        public override string TestName => "Bogie Transforms";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            var result = Pass();

            if (!livery.FrontBogie.IsDefined())
            {
                result.Fail($"{livery.id} - Front bogie type not defined", livery);
            }

            if (!livery.RearBogie.IsDefined())
            {
                result.Fail($"{livery.id} - Rear bogie type not defined", livery);
            }

            CheckBogie(livery.prefab!.transform.FindSafe(CarPartNames.Bogies.FRONT), true);
            CheckBogie(livery.prefab.transform.FindSafe(CarPartNames.Bogies.REAR), true);

            if (livery.parentType != null && livery.parentType.UseCustomGauge && !livery.UseCustomFrontBogie && !livery.UseCustomRearBogie)
            {
                result.Warning($"{livery.id} - Car uses custom gauge but livery uses default bogies", livery);
            }

            return result;

            void CheckBogie(Transform? bogie, bool isFront)
            {
                if (bogie == null)
                {
                    result.Fail($"{livery.id} - Missing {GetPosition(isFront)} bogie transform", livery);
                    return;
                }

                if (bogie.transform.position.y != 0)
                {
                    result.Fail($"{livery.id} - {GetName(isFront)} must be at Y = 0", bogie);
                }

                // Not a custom bogie, return.
                if (!(isFront ? livery.UseCustomFrontBogie : livery.UseCustomRearBogie)) return;

                var bogieCar = bogie.FindSafe(CarPartNames.Bogies.BOGIE_CAR);

                if (bogieCar == null)
                {
                    result.Fail($"{livery.id} - Missing {CarPartNames.Bogies.BOGIE_CAR} child for custom {GetPosition(isFront)} bogie", bogie);
                    return;
                }

                CheckMeshes(bogieCar);
            }

            void CheckMeshes(Transform bogieCar)
            {
                foreach (MeshFilter filter in bogieCar!.GetComponentsInChildren<MeshFilter>(true))
                {
                    if (filter.sharedMesh == null)
                    {
                        if (CCLEditorSettings.Settings.DisplayWarningsForMissingMeshesInBogies)
                        {
                            result.Warning($"{livery.id} - {filter.name} is missing a mesh", filter);
                        }
                    }
                    else if (!filter.sharedMesh.isReadable)
                    {
                        result.Warning($"{livery.id} - Mesh {filter.sharedMesh.name} on {filter.name} doesn't have Read/Write enabled", filter.sharedMesh);
                    }
                }
            }

            static string GetPosition(bool isFront)
            {
                return isFront ? "front" : "rear";
            }

            static string GetName(bool isFront)
            {
                return isFront ? CarPartNames.Bogies.FRONT : CarPartNames.Bogies.REAR;
            }
        }
    }
}
