using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL_GameScripts
{
    public class CargoModelSetup : MonoBehaviour
    {
        public BaseCargoType CargoType = BaseCargoType.Coal;
        public string CustomCargo = null;

        public GameObject Model = null;
        public string BaseModel = null;

        public override string ToString()
        {
            string cargo = !string.IsNullOrEmpty(CustomCargo) ? CustomCargo : CargoType.ToString();
            string model = !string.IsNullOrEmpty(BaseModel) ? BaseModel : (Model ? Model.name : "none");
            return $"[{cargo}, {model}]";
        }

        public void OnValidate()
        {
            if (!string.IsNullOrEmpty(CustomCargo))
            {
                CargoType = BaseCargoType.Custom;
            }

            if (Model)
            {
                BaseModel = null;
            }
        }

        public void ValidateColliders()
        {
            if (!Model || UnityEditor.PrefabUtility.IsPartOfPrefabAsset(Model)) return;

            // [colliders]
            Transform colliderRoot = Model.transform.Find(CarPartNames.COLLIDERS_ROOT);
            if (!colliderRoot)
            {
                // collider should be initialized in prefab, but make sure
                Debug.LogWarning($"Adding collision root to {Model.name}");

                GameObject colliders = new GameObject(CarPartNames.COLLIDERS_ROOT);
                colliderRoot = colliders.transform;
                colliderRoot.parent = Model.transform;
            }

            // [collision]
            Transform collision = colliderRoot.Find(CarPartNames.COLLISION_ROOT);
            if (!collision)
            {
                var collisionObj = new GameObject(CarPartNames.COLLISION_ROOT);
                collision = collisionObj.transform;
                collision.parent = colliderRoot.transform;
            }

            // find [walkable]
            // copy walkable to items if items doesn't exist
            Transform walkable = colliderRoot.Find(CarPartNames.WALKABLE_COLLIDERS);
            if (walkable)
            {
                // set layer
                ModelUtil.SetLayersRecursive(walkable.gameObject, DVLayer.Train_Walkable);

                Transform items = colliderRoot.Find(CarPartNames.ITEM_COLLIDERS);
                if (!items)
                {
                    GameObject newItemsObj = Instantiate(walkable.gameObject, colliderRoot);
                    newItemsObj.name = CarPartNames.ITEM_COLLIDERS;
                }

                //var boundingColliders = collision.GetComponentsInChildren<BoxCollider>();
                //if (boundingColliders.Length == 0)
                //{
                //    // autogenerate bounding box from walkable extents (only works with box collider bits though)
                //    var walkableColliders = walkable.GetComponentsInChildren<BoxCollider>();
                //    if (walkableColliders.Length > 0)
                //    {
                //        Debug.Log($"{Model.name} - Building bounding collision box from walkable colliders");

                //        Bounds boundBox = BoundsUtil.BoxColliderAABB(walkableColliders[0], Model.transform);
                //        for (int i = 1; i < walkableColliders.Length; i++)
                //        {
                //            boundBox.Encapsulate(BoundsUtil.BoxColliderAABB(walkableColliders[i], Model.transform));
                //        }

                //        BoxCollider newCollisionBox = collision.gameObject.AddComponent<BoxCollider>();
                //        newCollisionBox.center = boundBox.center - collision.localPosition;
                //        newCollisionBox.size = boundBox.size;
                //    }
                //}
            }
        }
    }
}
