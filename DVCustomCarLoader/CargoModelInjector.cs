using CCL_GameScripts;
using DV.Logic.Job;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DVCustomCarLoader
{
    public static class CargoModelInjector
    {
        private static readonly Dictionary<string, CargoContainerType> idToContainerType = new Dictionary<string, CargoContainerType>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<CargoContainerType, string> customCargoContainers = new Dictionary<CargoContainerType, string>();
        private static readonly Dictionary<string, GameObject> customModels = new Dictionary<string, GameObject>();

        private static Dictionary<TrainCarType, CargoContainerType> CarTypeToContainerType => CargoTypes.CarTypeToContainerType;
        private static readonly Dictionary<CargoType, List<CargoContainerType>> CargoTypeToSupportedContainer;
        private static readonly Dictionary<CargoContainerType, List<TrainCarType>> ContainerTypeToCarTypes;

        // Containers
        public static bool TryGetContainerTypeById(string id, out CargoContainerType type) => idToContainerType.TryGetValue(id.ToLower(), out type);
        public static CargoContainerType ContainerTypeById(string id) => idToContainerType[id];
        public static bool IsCustomTypeRegistered(CargoContainerType containerType) => customCargoContainers.ContainsKey(containerType);
        public static bool IsCustomTypeRegistered(string identifier) => idToContainerType.ContainsKey(identifier);

        public static bool TryGetContainerName(CargoContainerType containerType, out string name) =>
            customCargoContainers.TryGetValue(containerType, out name);

        public static bool TryGetModelByName(string id, out GameObject model) => customModels.TryGetValue(id, out model);

        public static bool IsInCustomRange(CargoContainerType containerType) => (int)containerType >= BaseInjector.CUSTOM_TYPE_OFFSET;

        static CargoModelInjector()
        {
            AccessTools.Field(typeof(CargoTypes), "cargoTypeToSupportedCarContainer").SaveTo(out CargoTypeToSupportedContainer);
            if (CargoTypeToSupportedContainer == null)
            {
                Main.Error("Failed to get CargoTypes.cargoTypeToSupportedCarContainer");
            }

            AccessTools.Property(typeof(CargoTypes), "ContainerTypeToCarTypes").SaveTo(out ContainerTypeToCarTypes);
            if (ContainerTypeToCarTypes == null)
            {
                Main.Error("Failed to get CargoTypes.ContainerTypeToCarTypes");
            }
        }

        public static void RegisterCargo(CustomCar car)
        {
            if (!car.CargoClass.IsCustomCargoClass())
            {
                CarTypeToContainerType.Add(car.CarType, car.CargoClass);
                ContainerTypeToCarTypes[car.CargoClass].Add(car.CarType);
                return;
            }

            car.CargoClass = BaseInjector.GenerateUniqueType<CargoContainerType>(car.identifier, IsCustomTypeRegistered);

            idToContainerType.Add(car.identifier, car.CargoClass);
            customCargoContainers.Add(car.CargoClass, car.identifier);

            CarTypeToContainerType.Add(car.CarType, car.CargoClass);
            ContainerTypeToCarTypes.Add(car.CargoClass, new List<TrainCarType>() { car.CarType });

            var modelDict = new Dictionary<CargoType, List<string>>();
            CargoModelsData.CargoContainerToAvailableCargoTypeModels.Add(car.CargoClass, modelDict);

            if (car.CargoModels != null && car.CargoModels.Any())
            {
                foreach (var model in car.CargoModels)
                {
                    // figure out cargo type
                    CargoType cargoType;
                    if (model.CargoType == BaseCargoType.Custom)
                    {
                        if (!CustomCargoInjector.TryGetCargoTypeById(model.CustomCargo, out cargoType))
                        {
                            Main.Error($"Cargo model for {car.identifier} references missing custom cargo {model.CustomCargo}");
                            continue;
                        }
                    }
                    else
                    {
                        cargoType = (CargoType)model.CargoType;
                    }

                    // register cargo to custom container
                    if (!CargoTypeToSupportedContainer.TryGetValue(cargoType, out var supportedContainers))
                    {
                        supportedContainers = new List<CargoContainerType>();
                        CargoTypeToSupportedContainer.Add(cargoType, supportedContainers);
                    }
                    supportedContainers.Add(car.CargoClass);

                    // figure out model
                    string modelName = model.BaseModel;
                    if (model.Model)
                    {
                        modelName = $"CCL_{car.identifier}_{cargoType.GetShortCargoName()}";
                        model.Model.name = modelName;

                        //ValidateCargoColliders(model.Model);

                        customModels.Add(modelName, model.Model);
                    }

                    bool isModelNamePresent = !string.IsNullOrEmpty(modelName);
                    if (isModelNamePresent)
                    {
                        if (!modelDict.TryGetValue(cargoType, out var nameList))
                        {
                            nameList = new List<string>();
                            modelDict.Add(cargoType, nameList);
                        }

                        nameList.Add(modelName);
                    }

                    Main.LogVerbose($"Cargo {cargoType.GetShortCargoName()} - {car.identifier} - {(isModelNamePresent ? modelName : "none")}");
                }
            }
        }

        private static void ValidateCargoColliders(GameObject model)
        {
            // [colliders]
            Transform colliderRoot = model.transform.Find(CarPartNames.COLLIDERS_ROOT);
            if (!colliderRoot)
            {
                // collider should be initialized in prefab, but make sure
                Main.Warning($"Adding collision root to {model.name}, should have been part of prefab!");

                GameObject colliders = new GameObject(CarPartNames.COLLIDERS_ROOT);
                colliderRoot = colliders.transform;
                colliderRoot.parent = model.transform;
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
                Transform items = colliderRoot.Find(CarPartNames.ITEM_COLLIDERS);
                if (!items)
                {
                    Main.LogVerbose("Reusing walkable colliders as item colliders");
                    GameObject newItemsObj = UnityEngine.Object.Instantiate(walkable.gameObject, colliderRoot);
                    newItemsObj.name = CarPartNames.ITEM_COLLIDERS;
                }

                // set layer
                walkable.gameObject.SetLayersRecursive("Train_Walkable");

                var boundingColliders = collision.GetComponentsInChildren<BoxCollider>();
                if (boundingColliders.Length == 0)
                {
                    // autogenerate bounding box from walkable extents (only works with box collider bits though)
                    var walkableColliders = walkable.GetComponentsInChildren<BoxCollider>();
                    if (walkableColliders.Length > 0)
                    {
                        Main.LogVerbose("Building bounding collision box from walkable colliders");

                        Bounds boundBox = BoundsUtil.BoxColliderAABB(walkableColliders[0], model.transform);
                        for (int i = 1; i < walkableColliders.Length; i++)
                        {
                            boundBox.Encapsulate(BoundsUtil.BoxColliderAABB(walkableColliders[i], model.transform));
                        }

                        BoxCollider newCollisionBox = collision.gameObject.AddComponent<BoxCollider>();
                        newCollisionBox.center = boundBox.center - collision.localPosition;
                        newCollisionBox.size = boundBox.size;
                    }
                }
            }
        }
    }

#warning Don't commit this cargo visibility debugging code region.
    #region Cargo Visibility Debug
    [HarmonyPatch(typeof(CargoModelsData), nameof(CargoModelsData.IsCargoVisibleOnCar))]
    public static class CargoVisible_Patch
    {
        public static void Postfix(TrainCarType carType, CargoType cargoType, ref bool __result)
        {
            if (!CarTypeInjector.IsInCustomRange(carType)) { return; }

            CargoContainerType containerType;
            Dictionary<CargoType, List<string>> dictionary;
            bool isContainerTypeRetrieved = CargoTypes.CarTypeToContainerType.TryGetValue(carType, out containerType);
            if (isContainerTypeRetrieved && CargoModelsData.CargoContainerToAvailableCargoTypeModels.TryGetValue(containerType, out dictionary))
            {
                bool isVisible = dictionary.ContainsKey(cargoType);
                string dictEntry = isVisible ? $"List{{{dictionary[cargoType].Join()}}}" : "None";
                Main.LogVerbose($"IsCargoVisibleOnCar? (carType: {carType}, cargoType: {cargoType}) -> {__result}\n\tcontainerType: {CargoTypes.DisplayName(containerType)}\n\tdictionaryEntry: {dictEntry}\n\tlocalIsVisible: {isVisible}");
            }
            else
            {
                Main.LogVerbose($"IsCargoVisibleOnCar? (carType: {carType}, cargoType: {cargoType}) -> {__result}\n\tCouldn't retrieve {(isContainerTypeRetrieved ? "ContainerType" : "Dictionary")}.");
            }
        }
    }
    #endregion

    [HarmonyPatch(typeof(CargoModelController))]
    public static class CargoModelController_Patches
    {
        [HarmonyPatch("InstantiateCargoModel")]
        [HarmonyPrefix]
        public static bool InstantiateCargoModel(string cargoPrefabName, out GameObject cargoModel,
            TrainCar ___trainCar, TrainCarColliders ___trainColliders, ref GameObject ___currentCargoCollisionCollidersRoot)
        {
            if (CargoModelInjector.TryGetModelByName(cargoPrefabName, out var model))
            {
                cargoModel = UnityEngine.Object.Instantiate(model, ___trainCar.interior.transform, false);
                cargoModel.transform.localPosition = Vector3.zero;
                cargoModel.transform.localRotation = Quaternion.identity;
                ___trainColliders.SetupCargo(cargoModel);
                ___currentCargoCollisionCollidersRoot = ___trainColliders.GetCargoCollision().gameObject;
                return false;
            }

            cargoModel = null;
            return true;
        }
    }

    [HarmonyPatch(typeof(CargoTypes), nameof(CargoTypes.DisplayName))]
    public static class ContainerDisplayName_Patch
    {
        public static bool Prefix(CargoContainerType containerType, ref string __result)
        {
            if (CargoModelInjector.TryGetContainerName(containerType, out string name))
            {
                __result = name;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(CargoTypes), nameof(CargoTypes.GetCarContainerTypesThatSupportCargoType))]
    public static class CargoTypes_GetContainersByCargo_Patch
    {
        public static void Postfix(ref List<CargoContainerType> __result)
        {
            if (Main.Settings.PreferCustomCargoContainersForJobs)
            {
                // override all base types
                if (__result.Any(CargoModelInjector.IsInCustomRange))
                {
                    __result = __result.Where(CargoModelInjector.IsInCustomRange).ToList();
                }
            }
        }
    }
}
