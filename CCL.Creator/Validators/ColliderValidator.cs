using CCL.Types;
using System;
using System.Linq;
using UnityEngine;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class ColliderValidator : LiveryValidator
    {
        public override string TestName => "Colliders";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            // root
            var collidersRoot = livery.prefab!.transform.FindSafe(CarPartNames.COLLIDERS_ROOT);
            if (!collidersRoot)
            {
                return Fail($"{livery.id} - {CarPartNames.COLLIDERS_ROOT} root is missing entirely!");
            }

            var result = Pass();

            // bounding collider
            var collision = collidersRoot.FindSafe(CarPartNames.COLLISION_ROOT);
            var collisionComp = collision ? collision!.GetComponentsInChildren<BoxCollider>(true) : Enumerable.Empty<Collider>();
            if (!collision || !collisionComp.Any())
            {
                result.Warning($"{livery.id} - Bounding {CarPartNames.COLLISION_ROOT} collider will be auto-generated");
            }

            // walkable
            var walkable = collidersRoot.FindSafe(CarPartNames.WALKABLE_COLLIDERS);
            var walkableComp = walkable ? walkable!.GetComponentsInChildren<Collider>(true) : Enumerable.Empty<Collider>();
            if (!walkable || !walkableComp.Any())
            {
                result.Fail($"{livery.id} - No {CarPartNames.WALKABLE_COLLIDERS} colliders set - car has no player collision");
            }

            // bogies
            var bogies = collidersRoot.FindSafe(CarPartNames.BOGIE_COLLIDERS);
            var bogieColliders = bogies ? bogies!.GetComponentsInChildren<Collider>(true) : Array.Empty<Collider>();
            if (!bogies || bogieColliders.Length != 2)
            {
                result.Fail($"{livery.id} - Incorrect number of {CarPartNames.BOGIE_COLLIDERS} colliders - should be 2");
            }

            return result;
        }
    }
}
