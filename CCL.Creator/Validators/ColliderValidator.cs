using CCL.Types;
using CCL.Types.Components;
using CCL.Types.Proxies;
using CCL.Types.Proxies.Customization;
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
            var collision = collidersRoot.Find(CarPartNames.Colliders.COLLISION);
            var collisionComp = collision ? collision!.GetComponentsInChildren<Collider>(true) : Array.Empty<Collider>();
            if (collision == null || collisionComp.Length < 1)
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
            if (collision != null)
            {
                CheckLocalOrigin(collision);

                if (collision.GetComponentsInChildren<MeshCollider>().Any(x => !x.convex))
                {
                    result.Fail("Non-convex mesh colliders are not supported for car collisions");
                }
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
                CheckLocalOrigin(walkable);

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
            if (items != null)
            {
                CheckLocalOrigin(items);

                foreach (var disabler in items.GetComponentsInChildren<DrillingDisablerProxy>(true))
                {
                    if (!HasDrillDisablerParent(disabler))
                    {
                        result.Warning($"{livery.id} - Drill disabler {disabler.name} is not under {CarPartNames.Colliders.DRILLING_DISABLERS}");
                    }
                }
            }
            else
            {
                result.Warning($"{livery.id} - {CarPartNames.Colliders.ITEMS} missing, will be copied from {CarPartNames.Colliders.WALKABLE}");
            }

            // Camera dampening.
            var cameraDampening = collidersRoot.FindSafe(CarPartNames.Colliders.CAMERA_DAMPENING);
            if (cameraDampening != null)
            {
                CheckLocalOrigin(cameraDampening);
            }

            // Bogies.
            var bogies = collidersRoot.FindSafe(CarPartNames.Colliders.BOGIES);
            var bogieColliders = bogies ? bogies!.GetComponentsInChildren<Collider>(true) : Array.Empty<Collider>();
            if (!bogies || bogieColliders.Length != 2)
            {
                result.Fail($"{livery.id} - Incorrect number of {CarPartNames.Colliders.BOGIES} colliders - should be 2", collidersRoot);
            }

            return result;

            void CheckLocalOrigin(Transform t)
            {
                if (InvalidOrigin(t))
                {
                    result.Warning($"{livery.id} - {t.name} is not at the local origin");
                }
            }
        }

        private static bool InvalidOrigin(Transform t)
        {
            return t.localPosition != Vector3.zero || t.localRotation != Quaternion.identity || t.localScale != Vector3.one;
        }

        private static bool HasDrillDisablerParent(DrillingDisablerProxy disabler)
        {
            var t = disabler.transform;

            while (t.parent != null)
            {
                t = t.parent;

                if (t.name == CarPartNames.Colliders.DRILLING_DISABLERS) return true;
            }

            return false;
        }
    }
}
