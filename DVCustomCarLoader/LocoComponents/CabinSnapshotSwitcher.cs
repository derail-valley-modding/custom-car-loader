using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CabinSnapshotSwitcher : InternalExternalSnapshotSwitcher
    {
        protected TrainCar AttachedTrainCar;
        protected DoorAndWindowTracker OpeningTracker;

        protected List<BoxCollider> interiorRegions = new List<BoxCollider>();

        protected static readonly MethodInfo PointInOABBMethod;
        protected readonly Func<Vector3, BoxCollider, bool> PointInOABB;

        static CabinSnapshotSwitcher()
        {
            PointInOABBMethod = AccessTools.Method(typeof(InternalExternalSnapshotSwitcher), "PointInOABB");
            if (PointInOABBMethod == null)
            {
                Main.Error("CabinSnapshotSwitcher - PointInOABB not found");
            }
        }

        public CabinSnapshotSwitcher()
        {
            PointInOABB = AccessTools.MethodDelegate<Func<Vector3, BoxCollider, bool>>(PointInOABBMethod, this);
        }

        public void AddCabRegion(BoxCollider box)
        {
            if (interiorRegions.Count == 0)
            {
                this.box = box;
            }
            interiorRegions.Add(box);
        }

        protected override void Start()
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

        protected override bool InInternalSpace(Transform playerCameraTransform)
        {
            return OpeningTracker && !OpeningTracker.IsAnythingOpen() && interiorRegions.Any(r => PointInOABB(playerCameraTransform.position, r));
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