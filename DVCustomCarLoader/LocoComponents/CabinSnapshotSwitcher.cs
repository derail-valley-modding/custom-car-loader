using System.Collections;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public class CabinSnapshotSwitcher : InternalExternalSnapshotSwitcher
    {
        protected TrainCar AttachedTrainCar;
        protected DoorAndWindowTracker OpeningTracker;

        protected override void Start()
        {
            base.Start();
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
            return base.InInternalSpace(playerCameraTransform) && OpeningTracker && !OpeningTracker.IsAnythingOpen();
        }
    }
}