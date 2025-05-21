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
            if (car.CargoSetup == null || car.CargoSetup.IsEmpty)
            {
                return Skip();
            }

            var result = Pass();
            var hashId = new HashSet<string>();

            for (int i = 0; i < car.CargoSetup.Entries.Count; i++)
            {
                var cargo = car.CargoSetup.Entries[i];

                if (cargo.AmountPerCar <= 0)
                {
                    result.Fail("Cannot have 0 or negative cargo amount per car");
                }

                if (string.IsNullOrWhiteSpace(cargo.CargoId))
                {
                    result.Fail("Cargo ID is empty");
                }
                else
                {
                    if (hashId.Contains(cargo.CargoId))
                    {
                        result.Warning($"Repeated instance of cargo '{cargo.CargoId}'");
                    }
                    else
                    {
                        hashId.Add(cargo.CargoId);
                    }
                }

                if (cargo.Models != null)
                {
                    foreach (var model in cargo.Models)
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
