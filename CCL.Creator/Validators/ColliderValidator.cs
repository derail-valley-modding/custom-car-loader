using CCL.Types;
using CCL.Types.Components;
using CCL.Types.Proxies;
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
            // Root.
            var collidersRoot = livery.prefab!.transform.FindSafe(CarPartNames.Colliders.ROOT);
            if (collidersRoot == null)
            {
                return Fail($"{livery.id} - {CarPartNames.Colliders.ROOT} root is missing entirely!");
            }

            var result = Pass();

            if (InvalidOrigin(collidersRoot))
            {
                result.Warning($"{livery.id} - {CarPartNames.Colliders.ROOT} is not at the local origin");
            }

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
                !collisionComp.Any(x => x.GetComponent<ServiceCollider>()))
            {
                result.Warning($"{livery.id} - No collider with the {nameof(ServiceCollider)} component found, " +
                    $"vehicle cannot be serviced!", collidersRoot);
            }
            if (collision != null && InvalidOrigin(collision))
            {
                result.Warning($"{livery.id} - {CarPartNames.Colliders.COLLISION} is not at the local origin");
            }

            // Walkable.
            var walkable = collidersRoot.FindSafe(CarPartNames.Colliders.WALKABLE);
            var walkableComp = walkable ? walkable!.GetComponentsInChildren<Collider>(true) : Array.Empty<Collider>();
            if (!walkable || !walkableComp.Any())
            {
                result.Fail($"{livery.id} - No {CarPartNames.Colliders.WALKABLE} colliders set - car has no player collision", collidersRoot);
            }

            if (walkable != null)
            {
                if (InvalidOrigin(walkable))
                {
                    result.Warning($"{livery.id} - {CarPartNames.Colliders.WALKABLE} is not at the local origin");
                }

                // Fall safeties.
                foreach (Collider collider in walkableComp)
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


            // Items.
            var items = collidersRoot.FindSafe(CarPartNames.Colliders.ITEMS);
            if (items != null && InvalidOrigin(items))
            {
                result.Warning($"{livery.id} - {CarPartNames.Colliders.ITEMS} is not at the local origin");
            }

            // Camera dampening.
            var cameraDampening = collidersRoot.FindSafe(CarPartNames.Colliders.CAMERA_DAMPENING);
            if (cameraDampening != null && InvalidOrigin(cameraDampening))
            {
                result.Warning($"{livery.id} - {CarPartNames.Colliders.CAMERA_DAMPENING} is not at the local origin");
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

        private static bool InvalidOrigin(Transform t)
        {
            return t.localPosition != Vector3.zero || t.localRotation != Quaternion.identity || t.localScale != Vector3.one;
        }
    }
}
