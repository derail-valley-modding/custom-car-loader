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

            var bogieF = livery.prefab!.transform.FindSafe(CarPartNames.Bogies.FRONT);
            if (!bogieF)
            {
                result.Fail($"{livery.id} - Missing front bogie transform");
            }
            else
            {
                if (bogieF!.transform.position.y != 0)
                {
                    result.Fail($"{livery.id} - BogieF must be at y=0");
                }

                if (livery.UseCustomFrontBogie)
                {
                    var bogieCar = bogieF.FindSafe(CarPartNames.Bogies.BOGIE_CAR);
                    if (!bogieCar)
                    {
                        result.Fail($"{livery.id} - Missing {CarPartNames.Bogies.BOGIE_CAR} child for custom front bogie");
                    }
                    else
                    {
                        foreach (MeshFilter filter in bogieCar!.GetComponentsInChildren<MeshFilter>(true))
                        {
                            if (filter.sharedMesh == null)
                            {
                                result.Warning($"{livery.id} - {filter.name} is missing a mesh");
                            }
                            else if (!filter.sharedMesh.isReadable)
                            {
                                result.Warning($"{livery.id} - Mesh {filter.sharedMesh.name} on {filter.name} doesn't have Read/Write enabled");
                            }
                        }
                    }
                }
            }

            var bogieR = livery.prefab.transform.FindSafe(CarPartNames.Bogies.REAR);
            if (!bogieR)
            {
                result.Fail($"{livery.id} - Missing rear bogie transform");
            }
            else
            {
                if (bogieR!.transform.position.y != 0)
                {
                    result.Fail($"{livery.id} - BogieR must be at y=0");
                }

                if (livery.UseCustomRearBogie)
                {
                    var bogieCar = bogieR.FindSafe(CarPartNames.Bogies.BOGIE_CAR);
                    if (!bogieCar)
                    {
                        result.Fail($"{livery.id} - Missing {CarPartNames.Bogies.BOGIE_CAR} child for custom rear bogie");
                    }
                    else
                    {
                        foreach (MeshFilter filter in bogieCar!.GetComponentsInChildren<MeshFilter>(true))
                        {
                            if (filter.sharedMesh == null)
                            {
                                result.Warning($"{livery.id} - {filter.name} is missing a mesh");
                            }
                            else if (!filter.sharedMesh.isReadable)
                            {
                                result.Warning($"{livery.id} - Mesh {filter.sharedMesh.name} on {filter.name} doesn't have Read/Write enabled");
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
