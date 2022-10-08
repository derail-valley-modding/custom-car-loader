using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CabinSnapshotSwitcher : MonoBehaviour
    {
        private static readonly AccessTools.FieldRef<object, int> UpdatedInternalityLastFrame;
        private static readonly AccessTools.FieldRef<object, bool> IsInsideAny;

        static CabinSnapshotSwitcher()
        {
            UpdatedInternalityLastFrame = AccessTools.FieldRefAccess<int>(typeof(InternalExternalSnapshotSwitcher), "updatedInternalityLastFrame");
            IsInsideAny = AccessTools.FieldRefAccess<bool>(typeof(InternalExternalSnapshotSwitcher), "isInsideAny");
        }

        protected TrainCar AttachedTrainCar;
        protected DoorAndWindowTracker OpeningTracker;

        public List<Collider> interiorRegions = new List<Collider>();
        public List<int> cabIds = new List<int>();

        private bool isInside;

        public void AddCabRegion(int cabId, IEnumerable<Collider> region)
        {
            foreach (var collider in region)
            {
                interiorRegions.Add(collider);
                cabIds.Add(cabId);
                Main.LogVerbose($"Add collider {collider.GetType()} for cab {cabId}");
            }
        }

        protected void Start()
        {
            if (!AudioManager.e)
            {
                Main.Error("AudioManager instance not found. This should not happen. Disabling self.");
                enabled = false;
                return;
            }

            AttachedTrainCar = GetComponent<TrainCar>();
            if( !AttachedTrainCar )
            {
                Main.Error("Traincar not found for CabinSnapshotSwitcher");
                enabled = false;
                return;
            }

            AttachedTrainCar.InteriorPrefabLoaded += OnInteriorLoaded;
        }

        private void Update()
        {
            Camera playerCamera = PlayerManager.PlayerCamera;
            if (!playerCamera)
            {
                return;
            }

            int frameCount = Time.frameCount;
            if (UpdatedInternalityLastFrame() != frameCount)
            {
                AudioManager e = AudioManager.e;
                UpdatedInternalityLastFrame() = frameCount;

                if (IsInsideAny() != (e.internality == 1f))
                {
                    e.internality = (IsInsideAny() ? 1f : 0f);
                }
                IsInsideAny() = false;
            }

            if ((transform.position - playerCamera.transform.position).sqrMagnitude > 36f && !isInside)
            {
                return;
            }

            isInside = InInternalSpace(playerCamera.transform);
            IsInsideAny() |= isInside;
        }

        private void OnDestroy()
        {
            if (UnloadWatcher.isUnloading) return;

            if (AttachedTrainCar)
            {
                AttachedTrainCar.InteriorPrefabLoaded -= OnInteriorLoaded;
            }
        }

        protected virtual void OnInteriorLoaded(GameObject interior)
        {
            if (interior)
            {
                OpeningTracker = interior.GetComponent<DoorAndWindowTracker>();
                if (!OpeningTracker)
                {
                    Main.Error($"No DoorAndWindowTracker found on {AttachedTrainCar.ID}");
                    return;
                }
            }
        }

        private static bool IsPointInsideBoxBounds(Vector3 point, BoxCollider box)
        {
            point = box.transform.InverseTransformPoint(point) - box.center;
            float num = box.size.x * 0.5f;
            float num2 = box.size.y * 0.5f;
            float num3 = box.size.z * 0.5f;
            return point.x < num && point.x > -num && point.y < num2 && point.y > -num2 && point.z < num3 && point.z > -num3;
        }

        protected static bool IsPointInsideCollider(Vector3 point, Collider collider)
        {
            if (collider is BoxCollider box)
            {
                return IsPointInsideBoxBounds(point, box);
            }
            else
            {
                return collider.ClosestPoint(point) == point;
            }
        }

        protected bool InInternalSpace(Transform playerCameraTransform)
        {
            for (int i = 0; i < interiorRegions.Count; i++)
            {
                var collider = interiorRegions[i];

                if (IsPointInsideCollider(playerCameraTransform.position, collider))
                {
                    if (!OpeningTracker) return true;

                    int cabId = cabIds[i];
                    return !OpeningTracker.IsAnythingOpen(cabId);
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CabTeleportDestination))]
    public static class CabTeleportDestinationPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnEnable")]
        public static bool OnEnable(CabTeleportDestination __instance)
        {
            return __instance.hoverRenderer;
        }

        [HarmonyPrefix]
        [HarmonyPatch("OnDisable")]
        public static bool OnDisable(CabTeleportDestination __instance)
        {
            return __instance.hoverRenderer;
        }
    }
}