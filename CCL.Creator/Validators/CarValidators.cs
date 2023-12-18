using CCL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Validators
{
    internal static class CarValidators
    {
        private static IEnumerable<Result> CheckEachOrPass<T>(IEnumerable<T> items, Func<T, Result?> check)
        {
            bool any = false;
            foreach (var item in items)
            {
                if (check(item) is Result result)
                {
                    any = true;
                    yield return result;
                }
            }

            if (!any)
            {
                yield return Result.Pass();
            }
        }

        private static IEnumerable<Result> CheckEachOrPass<T>(IEnumerable<T> items, Func<T, IEnumerable<Result>> check)
        {
            bool any = false;
            foreach (var item in items)
            {
                foreach (var result in check(item))
                {
                    any = true;
                    yield return result;
                }
            }

            if (!any)
            {
                yield return Result.Pass();
            }
        }

        [CarValidator("Car Setup")]
        public static IEnumerable<Result> CheckCarSetup(CustomCarType carType)
        {
            return CheckEachOrPass(carType.liveries, livery =>
            {
                if (!livery.prefab)
                {
                    return Result.Critical("Livery must have a prefab assigned!");
                }
                if (livery.BaseCarType == BaseTrainCarType.NotSet)
                {
                    return Result.Failed("Base car type must be set");
                }
                return null;
            });
        }

        [CarValidator("Root Transform")]
        public static IEnumerable<Result> CheckTransform(CustomCarType carType)
        {
            return CheckEachOrPass(carType.liveries, livery =>
            {
                if (livery.prefab!.transform.position != Vector3.zero)
                {
                    return Result.Failed($"{livery.id} - Not at (0,0,0)");
                }
                if (livery.prefab.transform.eulerAngles != Vector3.zero)
                {
                    return Result.Failed($"{livery.id} - Non-zero rotation");
                }
                if (livery.prefab.transform.localScale != Vector3.one)
                {
                    return Result.Failed($"{livery.id} - Scale is not 1");
                }
                return null;
            });
        }

        [CarValidator("LOD Group")]
        public static IEnumerable<Result> CheckLOD(CustomCarType carType)
        {
            return CheckEachOrPass(carType.liveries, livery =>
            {
                var lodGroup = livery.prefab!.GetComponent<LODGroup>();
                if (lodGroup)
                {
                    foreach (var lod in lodGroup.GetLODs())
                    {
                        if (lod.renderers.Length == 0)
                        {
                            return Result.Warning("Missing renderers on LOD");
                        }
                    }
                }
                return null;
            });
        }

        [CarValidator("Bogie Transforms")]
        public static IEnumerable<Result> CheckBogies(CustomCarType carType)
        {
            return CheckEachOrPass(carType.liveries, CheckLiveryBogies);
        }

        private static IEnumerable<Result> CheckLiveryBogies(CustomCarVariant livery)
        {
            var bogieF = livery.prefab!.transform.FindSafe(CarPartNames.BOGIE_FRONT);
            if (!bogieF)
            {
                yield return Result.Failed($"{livery.id} - Missing front bogie transform");
            }
            else
            {
                if (bogieF!.transform.position.y != 0)
                {
                    yield return Result.Failed($"{livery.id} - BogieF must be at y=0");
                }

                if (livery.UseCustomFrontBogie)
                {
                    var bogieCar = bogieF.FindSafe(CarPartNames.BOGIE_CAR);
                    if (!bogieCar)
                    {
                        yield return Result.Failed($"{livery.id} - Missing {CarPartNames.BOGIE_CAR} child for custom front bogie");
                    }
                    else
                    {
                        foreach (MeshFilter filter in bogieCar!.GetComponentsInChildren<MeshFilter>(true))
                        {
                            if (filter.sharedMesh == null)
                            {
                                yield return Result.Warning($"{livery.id} - {filter.name} is missing a mesh");
                            }
                            else if (!filter.sharedMesh.isReadable)
                            {
                                yield return Result.Warning($"{livery.id} - Mesh {filter.sharedMesh.name} on {filter.name} doesn't have Read/Write enabled");
                            }
                        }
                    }
                }
            }

            var bogieR = livery.prefab.transform.FindSafe(CarPartNames.BOGIE_REAR);
            if (!bogieR)
            {
                yield return Result.Failed($"{livery.id} - Missing rear bogie transform");
            }
            else
            {
                if (bogieR!.transform.position.y != 0)
                {
                    yield return Result.Failed($"{livery.id} - BogieR must be at y=0");
                }

                if (livery.UseCustomRearBogie)
                {
                    var bogieCar = bogieR.FindSafe(CarPartNames.BOGIE_CAR);
                    if (!bogieCar)
                    {
                        yield return Result.Failed($"{livery.id} - Missing {CarPartNames.BOGIE_CAR} child for custom rear bogie");
                    }
                    else
                    {
                        foreach (MeshFilter filter in bogieCar!.GetComponentsInChildren<MeshFilter>(true))
                        {
                            if (filter.sharedMesh == null)
                            {
                                yield return Result.Warning($"{livery.id} - {filter.name} is missing a mesh");
                            }
                            else if (!filter.sharedMesh.isReadable)
                            {
                                yield return Result.Warning($"{livery.id} - Mesh {filter.sharedMesh.name} on {filter.name} doesn't have Read/Write enabled");
                            }
                        }
                    }
                }
            }
        }

        [CarValidator("Colliders")]
        public static IEnumerable<Result> CheckColliders(CustomCarType carType)
        {
            return CheckEachOrPass(carType.liveries, CheckLiveryColliders);
        }

        private static IEnumerable<Result> CheckLiveryColliders(CustomCarVariant livery)
        {
            // root
            var collidersRoot = livery.prefab!.transform.FindSafe(CarPartNames.COLLIDERS_ROOT);
            if (!collidersRoot)
            {
                yield return Result.Failed($"{livery.id} - {CarPartNames.COLLIDERS_ROOT} root is missing entirely!");
                yield break;
            }

            // bounding collider
            var collision = collidersRoot.FindSafe(CarPartNames.COLLISION_ROOT);
            var collisionComp = collision ? collision!.GetComponent<BoxCollider>() : null;
            if (!(collision && collisionComp))
            {
                yield return Result.Warning($"{livery.id} - Bounding {CarPartNames.COLLISION_ROOT} collider will be auto-generated");
            }

            // walkable
            var walkable = collidersRoot.FindSafe(CarPartNames.WALKABLE_COLLIDERS);
            var walkableComp = walkable ? walkable!.GetComponentsInChildren<Collider>() : Enumerable.Empty<Collider>();
            if (!walkable || !walkableComp.Any())
            {
                yield return Result.Failed($"{livery.id} - No {CarPartNames.WALKABLE_COLLIDERS} colliders set - car has no player collision");
            }

            // bogies
            var bogies = collidersRoot.FindSafe(CarPartNames.BOGIE_COLLIDERS);
            var bogieColliders = bogies ? bogies!.GetComponentsInChildren<Collider>() : Array.Empty<Collider>();
            if (!bogies || bogieColliders.Length != 2)
            {
                yield return Result.Failed($"{livery.id} - Incorrect number of {CarPartNames.BOGIE_COLLIDERS} colliders - should be 2");
            }
        }

        [CarValidator("Couplers & Buffers")]
        public static IEnumerable<Result> CheckCouplers(CustomCarType carType)
        {
            return CheckEachOrPass(carType.liveries, CheckLiveryCouplers);
        }

        private static IEnumerable<Result> CheckLiveryCouplers(CustomCarVariant livery)
        {
            var frontRig = livery.prefab!.transform.FindSafe(CarPartNames.COUPLER_RIG_FRONT);
            if (!frontRig)
            {
                yield return Result.Failed("Missing front coupler rig " + CarPartNames.COUPLER_RIG_FRONT);
            }
            else
            {
                if (frontRig!.position.x != 0 || frontRig.position.y != 1.05f)
                {
                    yield return Result.Warning("Front coupler rig should be at x = 0, y = 1.05");
                }

                if (livery.UseCustomBuffers)
                {
                    foreach (string name in CarPartNames.BUFFER_FRONT_PADS)
                    {
                        var pad = frontRig.FindSafe(name);
                        if (!pad)
                        {
                            yield return Result.Warning("Missing buffer pad " + name);
                        }
                    }
                }
            }

            var rearRig = livery.prefab.transform.FindSafe(CarPartNames.COUPLER_RIG_REAR);
            if (!rearRig)
            {
                yield return Result.Failed("Missing rear coupler rig " + CarPartNames.COUPLER_RIG_REAR);
            }
            else
            {
                if (rearRig!.position.x != 0 || rearRig.position.y != 1.05f)
                {
                    yield return Result.Warning("Rear coupler rig should be at x = 0, y = 1.05");
                }

                if (livery.UseCustomBuffers)
                {
                    foreach (string name in CarPartNames.BUFFER_REAR_PADS)
                    {
                        var pad = rearRig.FindSafe(name);
                        if (!pad)
                        {
                            yield return Result.Warning("Missing buffer pad " + name);
                        }
                    }
                }
            }
        }

        [CarValidator("Interior")]
        public static IEnumerable<Result> CheckInteriorTransform(CustomCarType carType)
        {
            return CheckEachOrPass(carType.liveries, livery =>
            {
                if (livery.interiorPrefab)
                {
                    if (livery.interiorPrefab!.transform.position != Vector3.zero)
                    {
                        return Result.Warning("Interior is not centered at (0,0,0)");
                    }
                }
                return null;
            });
        }

        [CarValidator("Cargo Settings")]
        public static IEnumerable<Result> CheckCargoSettings(CustomCarType carType)
        {
            if (carType.CargoTypes.IsEmpty)
            {
                yield return Result.Skipped();
                yield break;
            }

            bool any = false;
            foreach (var cargo in carType.CargoTypes.Entries)
            {
                if (cargo.AmountPerCar <= 0)
                {
                    any = true;
                    yield return Result.Failed("Cannot have 0 or negative cargo amount per car");
                }
                if (cargo.CargoType == BaseCargoType.None)
                {
                    any = true;
                    yield return Result.Failed("Cannot load cargo of type None");
                }

                if (cargo.ModelVariants != null)
                {
                    foreach (var result in cargo.ModelVariants.SelectMany(CheckModelVariant))
                    {
                        any = true;
                        yield return result;
                    }
                }
            }

            if (!any)
            {
                yield return Result.Pass();
            }
        }

        private static IEnumerable<Result> CheckModelVariant(GameObject model)
        {
            // check colliders
            var collidersRoot = model.transform.FindSafe(CarPartNames.COLLIDERS_ROOT);
            if (!collidersRoot)
            {
                yield return Result.Failed($"Cargo {model.name} model {CarPartNames.COLLIDERS_ROOT} root is missing");
                yield break;
            }

            // bounding collider
            var collision = collidersRoot.FindSafe(CarPartNames.COLLISION_ROOT);
            var collisionComp = collision ? collision!.GetComponentInChildren<Collider>() : null;

            if (!(collision && collisionComp))
            {
                yield return Result.Warning($"Cargo {model.name} bounding {CarPartNames.COLLISION_ROOT} collider is missing");
            }
        }

        [CarValidator("Project Settings")]
        public static IEnumerable<Result> CheckProjectSettings(CustomCarType _)
        {
            // Obsolete VR settings.
#pragma warning disable 0618

            bool anyIssues = false;

            if (!PlayerSettings.virtualRealitySupported)
            {
                anyIssues = true;
                yield return Result.Warning("VR support isn't enabled");
            }

            string[] sdks = PlayerSettings.GetVirtualRealitySDKs(BuildTargetGroup.Standalone);
            if (!sdks.Contains("Oculus"))
            {
                anyIssues = true;
                yield return Result.Warning("Oculus support isn't enabled");
            }
            if (!sdks.Contains("OpenVR"))
            {
                anyIssues = true;
                yield return Result.Warning("OpenVR support isn't enabled");
            }

            if (!PlayerSettings.singlePassStereoRendering)
            {
                anyIssues = true;
                yield return Result.Warning("VR Stereo Rendering Mode isn't set to Single Pass");
            }

            if (!anyIssues)
            {
                yield return Result.Pass();
            }

#pragma warning restore 0618
        }
    }
}
