using CCL.Creator.Utility;
using CCL.Types;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    public static class CarPrefabManipulators
    {
        private static readonly Quaternion InvertRotation = Quaternion.Euler(0, 180, 0);

        public static void AlignBogieColliders(CustomCarVariant carType)
        {
            if (!carType.prefab)
            {
                Debug.LogWarning("Car prefab is empty, can't align bogies!");
                return;
            }
            if (!carType.parentType)
            {
                Debug.LogWarning("Parent car type is not set, can't align bogies!");
                return;
            }

            float wheelRadius = carType.parentType!.wheelRadius;
            string prefabPath = AssetDatabase.GetAssetPath(carType.prefab);
            var tempPrefab = PrefabUtility.LoadPrefabContents(prefabPath);

            // Align front collider.
            var frontCollider = GetFrontBogieCollider(tempPrefab);
            var frontBogie = GetFrontBogie(tempPrefab);
            var axles = GetAxles(frontBogie);
            if (frontCollider != null && frontBogie != null)
            {
                // Bogie centre
                float z = frontBogie.localPosition.z;

                if (!carType.UseCustomFrontBogie)
                {
                    // If using a vanilla bogie, get the axle distance from it.
                    z += GetBogieOffset(carType.FrontBogie);
                }
                else if (axles.Length > 0)
                {
                    // Otherwise, get the frontmost axle for this bogie.
                    z = axles.Last().position.z;
                }

                var frontCenter = new Vector3(0, wheelRadius, z);
                frontCollider.radius = wheelRadius;
                frontCollider.center = Vector3.zero;
                frontCollider.transform.localPosition = frontCenter;
            }

            // Align rear collider.
            var rearCollider = GetRearBogieCollider(tempPrefab);
            var rearBogie = GetRearBogie(tempPrefab);
            axles = GetAxles(rearBogie);
            if (rearCollider != null && rearBogie != null)
            {
                float z = rearBogie.localPosition.z;

                if (!carType.UseCustomRearBogie)
                {
                    z -= GetBogieOffset(carType.RearBogie);
                }
                else if (axles.Length > 0)
                {
                    // Opposite to front bogie, rearmost axle.
                    z = axles.First().position.z;
                }

                var rearCenter = new Vector3(0, wheelRadius, z);
                rearCollider.radius = wheelRadius;
                rearCollider.center = Vector3.zero;
                rearCollider.transform.localPosition = rearCenter;
            }

            PrefabUtility.SaveAsPrefabAsset(tempPrefab, prefabPath);
            Debug.Log("Successfully aligned bogie colliders");
        }

        private static Transform? GetFrontBogie(GameObject prefab) => prefab.transform.FindSafe(CarPartNames.Bogies.FRONT);
        private static Transform? GetRearBogie(GameObject prefab) => prefab.transform.FindSafe(CarPartNames.Bogies.REAR);

        private const string BOGIE_COLLIDERS = CarPartNames.Colliders.ROOT + "/" + CarPartNames.Colliders.BOGIES;

        private static CapsuleCollider? GetFrontBogieCollider(GameObject prefab)
        {
            var bogies = prefab.transform.FindSafe(BOGIE_COLLIDERS);
            if (bogies)
            {
                var bogieColliders = bogies!.GetComponentsInChildren<CapsuleCollider>(true).OrderBy(c => c.center.z);
                return bogieColliders.LastOrDefault();
            }
            return null;
        }

        private static CapsuleCollider? GetRearBogieCollider(GameObject prefab)
        {
            var bogies = prefab.transform.FindSafe(BOGIE_COLLIDERS);
            if (bogies)
            {
                var bogieColliders = bogies!.GetComponentsInChildren<CapsuleCollider>().OrderBy(c => c.center.z);
                return bogieColliders.FirstOrDefault();
            }
            return null;
        }

        private static Transform[] GetAxles(Transform? bogie)
        {
            if (bogie == null)
            {
                return new Transform[0];
            }

            Transform? axleParent = bogie.transform.FindSafe(CarPartNames.Bogies.BOGIE_CAR);

            if (axleParent == null)
            {
                return new Transform[0];
            }

            List<Transform> axles = new List<Transform>();

            foreach (Transform t in axleParent)
            {
                if (t.name == CarPartNames.Bogies.AXLE)
                {
                    axles.Add(t);
                }
            }

            return axles.OrderBy(x => x.position.z).ToArray();
        }

        private static float GetBogieOffset(BogieType bogieType)
        {
            switch (bogieType)
            {
                case BogieType.Default:
                    return 1.00f;
                case BogieType.DE6:
                    return 2.03f;
                case BogieType.DH4:
                    return 1.10f;
                default:
                    return 0.00f;
            }
        }

        private static CustomCarVariant s_working = null!;

        public static void ResetCouplers(CustomCarVariant variant)
        {
            if (variant.prefab == null)
            {
                Debug.LogWarning("Car prefab is empty, can't reset couplers!");
                return;
            }

            s_working = variant;

            if (variant.prefab.transform.TryFind(CarPartNames.Couplers.RIG_FRONT, out var coupler))
            {
                AlignFrontCoupler(coupler);
            }
            else
            {
                Debug.LogError("Missing front coupler rig!");
            }

            if (variant.prefab.transform.TryFind(CarPartNames.Couplers.RIG_REAR, out coupler))
            {
                AlignRearCoupler(coupler);
            }
            else
            {
                Debug.LogError("Missing rear coupler rig!");
            }

            s_working = null!;
            AssetHelper.SaveAsset(variant.prefab);
        }

        private static void AlignFrontCoupler(Transform t)
        {
            SetPosition(t, CarPartNames.Buffers.PAD_FL, new Vector3(-0.8640631f, 0, 0.2150002f));
            SetPosition(t, CarPartNames.Buffers.PAD_FR, new Vector3(0.8640631f, 0, 0.2150002f));
            SetPosition(t, CarPartNames.Buffers.PLATE_FRONT, new Vector3(0, -0.07841396f, -0.3319998f), scale: Vector3.one * 0.81f);
            SetPosition(t, CarPartNames.Couplers.COUPLER_FRONT, new Vector3(0, 0, -0.2495f));

            if (SetPosition(t, CarPartNames.Buffers.CHAIN_REGULAR, new Vector3(0, 0, 0)))
            {
                SetUpChainPart(t.Find(CarPartNames.Buffers.CHAIN_REGULAR));
            }
        }

        private static void AlignRearCoupler(Transform t)
        {
            SetPosition(t, CarPartNames.Buffers.PAD_RL, new Vector3(-0.8640631f, 0, -0.2139997f), InvertRotation);
            SetPosition(t, CarPartNames.Buffers.PAD_RR, new Vector3(0.8640631f, 0, -0.2139997f), InvertRotation);
            SetPosition(t, CarPartNames.Buffers.PLATE_REAR, new Vector3(0, -0.07841396f, 0.3319998f), InvertRotation, Vector3.one * 0.81f);
            SetPosition(t, CarPartNames.Couplers.COUPLER_REAR, new Vector3(0, 0, -0.2495f), InvertRotation);

            if (SetPosition(t, CarPartNames.Buffers.CHAIN_REGULAR, new Vector3(0, 0, 0), InvertRotation))
            {
                SetUpChainPart(t.Find(CarPartNames.Buffers.CHAIN_REGULAR));
            }
        }

        private static bool SetPosition(Transform t, string path, Vector3 pos, Quaternion? rot = null, Vector3? scale = null)
        {
            if (t.TryFind(path, out var child))
            {
                child.localPosition = pos;
                child.localRotation = rot ?? Quaternion.identity;
                child.localScale = scale ?? Vector3.one;

                return true;
            }
            else if (s_working.UseCustomBuffers)
            {
                Debug.LogWarning($"Missing '{t.name}/{path}'!");
            }

            return false;
        }

        private static void SetUpChainPart(Transform t)
        {
            SetPosition(t, CarPartNames.Buffers.ANCHORS[0], new Vector3(-0.8640631f, 0, 0.3f));
            SetPosition(t, CarPartNames.Buffers.ANCHORS[1], new Vector3(0.8640631f, 0, 0.3f));

            if (SetPosition(t, CarPartNames.Couplers.HOSES_ROOT, Vector3.zero))
            {
                var hoses = t.Find(CarPartNames.Couplers.HOSES_ROOT);

                if (SetPosition(hoses, CarPartNames.Couplers.AIR_HOSE, new Vector3(-0.383f, -0.08700001f, -0.173f)))
                {
                    if (SetPosition(hoses, $"{CarPartNames.Couplers.AIR_HOSE}/cock valve",
                        new Vector3(-0.02579999f, -0.0277f, 0.0649f),
                        Quaternion.Euler(135, 0, 89.99999f),
                        Vector3.one * 0.07f))
                    {
                        SetPosition(hoses, $"{CarPartNames.Couplers.AIR_HOSE}/cock valve/CableBase model",
                            new Vector3(-1.45664f, -0.3699999f, 0.3111272f),
                            Quaternion.Euler(-180, -92.20001f, 89.99999f),
                            new Vector3(-14.28572f, 14.28572f, 14.28572f));
                    }
                }

                if (SetPosition(hoses, CarPartNames.Couplers.MU_CONNECTOR, new Vector3(0.383f, 0.11f, -0.1875f)))
                {
                    SetPosition(hoses, $"{CarPartNames.Couplers.AIR_HOSE}/HeadEndPowerConnectorBase", Vector3.zero, scale: Vector3.one * 100f);
                }
            }
        }
    }
}
