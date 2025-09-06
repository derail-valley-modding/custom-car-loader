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
            // Check colliders.
            var collidersRoot = model.transform.FindSafe(CarPartNames.Colliders.ROOT);
            if (collidersRoot == null)
            {
                result.Fail($"Cargo {model.name} - {CarPartNames.Colliders.ROOT} root is missing", model);
                return;
            }

            if (collidersRoot.localPosition != Vector3.zero)
            {
                result.Warning($"Cargo {model.name} - {CarPartNames.Colliders.ROOT} is not at the local origin", model);
            }

            // Bounding collider.
            var collision = collidersRoot.FindSafe(CarPartNames.Colliders.COLLISION);
            var collisionComp = collision ? collision!.GetComponentInChildren<Collider>() : null;

            if (!(collision && collisionComp))
            {
                result.Warning($"Cargo {model.name} bounding {CarPartNames.Colliders.COLLISION} collider is missing", collidersRoot);
            }
            else if (collision != null && InvalidOrigin(collision))
            {
                result.Warning($"Cargo {model.name} - {CarPartNames.Colliders.COLLISION} is not at the local origin", model);
            }

            // Walkable collider.
            var walkable = collidersRoot.FindSafe(CarPartNames.Colliders.WALKABLE);

            if (walkable != null && InvalidOrigin(walkable))
            {
                result.Warning($"Cargo {model.name} - {CarPartNames.Colliders.WALKABLE} is not at the local origin", model);
            }

            // Item collider.
            var items = collidersRoot.FindSafe(CarPartNames.Colliders.ITEMS);

            if (items != null && InvalidOrigin(items))
            {
                result.Warning($"Cargo {model.name} - {CarPartNames.Colliders.ITEMS} is not at the local origin", model);
            }
        }

        private static bool InvalidOrigin(Transform t)
        {
            return t.localPosition != Vector3.zero || t.localRotation != Quaternion.identity || t.localScale != Vector3.one;
        }
    }
}
