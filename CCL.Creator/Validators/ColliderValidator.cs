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
            var collidersRoot = livery.prefab!.transform.FindSafe(CarPartNames.Colliders.ROOT);
            if (!collidersRoot)
            {
                return Fail($"{livery.id} - {CarPartNames.Colliders.ROOT} root is missing entirely!");
            }

            var result = Pass();

            // bounding collider
            var collision = collidersRoot.FindSafe(CarPartNames.Colliders.COLLISION);
            var collisionComp = collision ? collision!.GetComponentsInChildren<BoxCollider>(true) : Enumerable.Empty<Collider>();
            if (!collision || !collisionComp.Any())
            {
                result.Warning($"{livery.id} - Bounding {CarPartNames.Colliders.COLLISION} collider will be auto-generated", collidersRoot);
            }

            // walkable
            var walkable = collidersRoot.FindSafe(CarPartNames.Colliders.WALKABLE);
            var walkableComp = walkable ? walkable!.GetComponentsInChildren<Collider>(true) : Enumerable.Empty<Collider>();
            if (!walkable || !walkableComp.Any())
            {
                result.Fail($"{livery.id} - No {CarPartNames.Colliders.WALKABLE} colliders set - car has no player collision", collidersRoot);
            }

            // bogies
            var bogies = collidersRoot.FindSafe(CarPartNames.Colliders.BOGIES);
            var bogieColliders = bogies ? bogies!.GetComponentsInChildren<Collider>(true) : Array.Empty<Collider>();
            if (!bogies || bogieColliders.Length != 2)
            {
                result.Fail($"{livery.id} - Incorrect number of {CarPartNames.Colliders.BOGIES} colliders - should be 2", collidersRoot);
            }

            return result;
        }
    }
}
