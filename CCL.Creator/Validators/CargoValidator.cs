using CCL.Types;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Creator.Validators
{
    internal class CargoValidator : CarValidator
    {
        public override string TestName => "Cargo Settings";

        public override ValidationResult Validate(CustomCarType car)
        {
            if (car.CargoTypes.IsEmpty)
            {
                return Skip();
            }

            var result = Pass();
            var hashCargo = new HashSet<BaseCargoType>();
            var hashId = new HashSet<string>();

            foreach (var cargo in car.CargoTypes.Entries)
            {
                if (cargo.AmountPerCar <= 0)
                {
                    result.Fail("Cannot have 0 or negative cargo amount per car");
                }
                if (cargo.CargoType == BaseCargoType.None)
                {
                    result.Fail("Cannot load cargo of type None");
                }
                else if (cargo.IsCustom)
                {
                    if (hashId.Contains(cargo.CustomCargoId))
                    {
                        result.Warning($"Repeated instance of custom cargo '{cargo.CustomCargoId}'");
                    }
                    else
                    {
                        hashId.Add(cargo.CustomCargoId);
                    }
                }
                else
                {
                    if (hashCargo.Contains(cargo.CargoType))
                    {
                        result.Warning($"Repeated instance of cargo '{cargo.CargoType}'");
                    }
                    else
                    {
                        hashCargo.Add(cargo.CargoType);
                    }
                }

                if (cargo.ModelVariants != null)
                {
                    foreach (var model in cargo.ModelVariants)
                    {
                        CheckModelVariant(result, model);
                    }
                }
            }

            return result;
        }

        private void CheckModelVariant(ValidationResult result, GameObject model)
        {
            // check colliders
            var collidersRoot = model.transform.FindSafe(CarPartNames.Colliders.ROOT);
            if (!collidersRoot)
            {
                result.Fail($"Cargo {model.name} model {CarPartNames.Colliders.ROOT} root is missing", model);
                return;
            }

            // bounding collider
            var collision = collidersRoot.FindSafe(CarPartNames.Colliders.COLLISION);
            var collisionComp = collision ? collision!.GetComponentInChildren<Collider>() : null;

            if (!(collision && collisionComp))
            {
                result.Warning($"Cargo {model.name} bounding {CarPartNames.Colliders.COLLISION} collider is missing", collidersRoot);
            }
        }
    }
}
