using CCL.Types;
using CCL.Types.Proxies;
using System;
using System.Linq;
using UnityEngine;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class ColliderValidator : LiveryValidator
    {
        private const string ServiceTag = "MainTriggerCollider";

        public override string TestName => "Colliders";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            // Root.
            var collidersRoot = livery.prefab!.transform.FindSafe(CarPartNames.Colliders.ROOT);
            if (!collidersRoot)
            {
                return Fail($"{livery.id} - {CarPartNames.Colliders.ROOT} root is missing entirely!");
            }

            var result = Pass();

            // Bounding collider.
            var collision = collidersRoot.FindSafe(CarPartNames.Colliders.COLLISION);
            var collisionComp = collision ? collision!.GetComponentsInChildren<BoxCollider>(true) : Array.Empty<Collider>();
            if (!collision || collisionComp.Length < 1)
            {
                result.Warning($"{livery.id} - Bounding {CarPartNames.Colliders.COLLISION} collider will be auto-generated", collidersRoot);
            }
            // For anything that isn't a generic car, check if it can be serviced.
            if (collisionComp.Length > 0 && livery.parentType != null &&
                livery.parentType.KindSelection != DVTrainCarKind.Car && 
                !collisionComp.Any(x => x.tag == ServiceTag))
            {
                result.Warning($"{livery.id} - No collider in {CarPartNames.Colliders.COLLISION} with the tag \"{ServiceTag}\" found, " +
                    $"it won't be possible to service this vehicle", collidersRoot);
            }

            // Walkable.
            var walkable = collidersRoot.FindSafe(CarPartNames.Colliders.WALKABLE);
            var walkableComp = walkable ? walkable!.GetComponentsInChildren<Collider>(true) : Enumerable.Empty<Collider>();
            if (!walkable || !walkableComp.Any())
            {
                result.Fail($"{livery.id} - No {CarPartNames.Colliders.WALKABLE} colliders set - car has no player collision", collidersRoot);
            }

            // Fall safeties.
            if (walkable != null)
            {
                foreach (Collider collider in walkable.GetComponentsInChildren<Collider>())
                {
                    if (collider.name.StartsWith(CarPartNames.Colliders.FALL_SAFETY) && !collider.name.Equals(CarPartNames.Colliders.FALL_SAFETY))
                    {
                        result.Warning($"Bad fall safety name '{collider.name}'", collider);
                    }

                    if (collider.name.Equals(CarPartNames.Colliders.FALL_SAFETY))
                    {
                        if (!collider.GetComponent<TeleportArcPassThroughProxy>())
                        {
                            result.Warning($"Missing {nameof(TeleportArcPassThroughProxy)} in {CarPartNames.Colliders.FALL_SAFETY} collider", collider);
                        }

                        if (collider.isTrigger == false)
                        {
                            result.Warning($"{CarPartNames.Colliders.FALL_SAFETY} is not set as trigger", collider);
                        }
                    }
                }
            }

            // Bogies.
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
