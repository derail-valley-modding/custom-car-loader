using CCL.Types;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    public static class CarPrefabManipulators
    {
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

            // align front collider
            var frontCollider = GetFrontBogieCollider(tempPrefab);
            var frontBogie = GetFrontBogie(tempPrefab);
            var axles = GetAxles(frontBogie);
            if (frontCollider && frontBogie)
            {
                // Bogie centre
                float z = frontBogie!.localPosition.z;

                if (!carType.UseCustomFrontBogie)
                {
                    // If using a vanilla bogie, get the axle distance from it.
                    z += GetBogieOffset(carType.BaseCarType);
                }
                else if (axles.Length > 0)
                {
                    // Otherwise, get the frontmost axle for this bogie.
                    z = axles.Last().position.z;
                }

                var frontCenter = new Vector3(0, wheelRadius, z);
                frontCollider!.center = frontCenter;
                frontCollider.radius = wheelRadius;
            }

            // align rear collider
            var rearCollider = GetRearBogieCollider(tempPrefab);
            var rearBogie = GetRearBogie(tempPrefab);
            axles = GetAxles(rearBogie);
            if (rearCollider)
            {
                float z = rearBogie!.localPosition.z;

                if (!carType.UseCustomRearBogie)
                {
                    z -= GetBogieOffset(carType.BaseCarType);
                }
                else if (axles.Length > 0)
                {
                    // Opposite to front bogie, rearmost axle.
                    z = axles.First().position.z;
                }

                var rearCenter = new Vector3(0, wheelRadius, z);
                rearCollider!.center = rearCenter;
                rearCollider.radius = wheelRadius;
            }

            PrefabUtility.SaveAsPrefabAsset(tempPrefab, prefabPath);
            Debug.Log("Successfully aligned bogie colliders");
        }

        private static Transform? GetFrontBogie(GameObject prefab) => prefab.transform.FindSafe(CarPartNames.BOGIE_FRONT);
        private static Transform? GetRearBogie(GameObject prefab) => prefab.transform.FindSafe(CarPartNames.BOGIE_REAR);

        private const string BOGIE_COLLIDERS = CarPartNames.COLLIDERS_ROOT + "/" + CarPartNames.BOGIE_COLLIDERS;

        private static CapsuleCollider? GetFrontBogieCollider(GameObject prefab)
        {
            var bogies = prefab.transform.FindSafe(BOGIE_COLLIDERS);
            if (bogies)
            {
                var bogieColliders = bogies!.GetComponentsInChildren<CapsuleCollider>().OrderBy(c => c.center.z);
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

            Transform? axleParent = bogie.transform.FindSafe(CarPartNames.BOGIE_CAR);

            if (axleParent == null)
            {
                return new Transform[0];
            }

            List<Transform> axles = new List<Transform>();

            foreach (Transform t in axleParent)
            {
                if (t.name == CarPartNames.AXLE)
                {
                    axles.Add(t);
                }
            }

            return axles.OrderBy(x => x.position.z).ToArray();
        }

        private static float GetBogieOffset(BaseTrainCarType carType)
        {
            switch (carType)
            {
                case BaseTrainCarType.LocoDiesel:
                case BaseTrainCarType.LocoDE6Slug:
                    return 2.03f;
                case BaseTrainCarType.LocoDH4:
                    return 1.10f;
                case BaseTrainCarType.NotSet:
                case BaseTrainCarType.LocoShunter:
                case BaseTrainCarType.LocoDM3:
                case BaseTrainCarType.LocoRailbus:
                case BaseTrainCarType.LocoS060:
                case BaseTrainCarType.LocoSteamHeavy:
                case BaseTrainCarType.HandCar:
                    return 0.00f;
                default:
                    return 1.00f;
            }
        }
    }
}
