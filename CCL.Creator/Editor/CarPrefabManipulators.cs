using CCL.Types;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Editor
{
    public static class CarPrefabManipulators
    {
        public static void AlignBogieColliders(CustomLivery carType)
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

            float wheelRadius = carType.parentType.wheelRadius;
            string prefabPath = AssetDatabase.GetAssetPath(carType.prefab);
            var tempPrefab = PrefabUtility.LoadPrefabContents(prefabPath);

            // align front collider
            var frontCollider = GetFrontBogieCollider(tempPrefab);
            var frontBogie = GetFrontBogie(tempPrefab);
            if (frontCollider && frontBogie)
            {
                var frontCenter = new Vector3(0, wheelRadius, frontBogie!.localPosition.z);
                frontCollider!.center = frontCenter;
                frontCollider.radius = wheelRadius;
            }

            // align rear collider
            var rearCollider = GetRearBogieCollider(tempPrefab);
            var rearBogie = GetRearBogie(tempPrefab);
            if (rearCollider)
            {
                var rearCenter = new Vector3(0, wheelRadius, rearBogie!.localPosition.z);
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
    }
}
