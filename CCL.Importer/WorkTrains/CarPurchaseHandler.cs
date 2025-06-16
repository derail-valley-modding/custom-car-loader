using CCL.Importer.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCL.Importer.WorkTrains
{
    internal class CarPurchaseHandler
    {
        public static readonly List<CCL_CarVariant> WorkTrainLiveries = new();
        public static Action<CCL_CarVariant>? LiveryUnlocked;

        private static List<CCL_CarVariant> s_unlocked = new();

        public static List<CCL_CarVariant> UnlockedLiveries => s_unlocked;
        public static List<CCL_CarVariant> LockedLiveries => WorkTrainLiveries.Except(UnlockedLiveries).ToList();

        public static bool Unlock(CCL_CarVariant carVariant)
        {
            if (s_unlocked.Contains(carVariant) || !WorkTrainLiveries.Contains(carVariant)) return false;

            var vehicles = CarSpawner.Instance.vehiclesWithoutGarage.ToList();
            vehicles.Add(carVariant);
            CarSpawner.Instance.vehiclesWithoutGarage = vehicles.ToArray();
            s_unlocked.Add(carVariant);
            LiveryUnlocked?.Invoke(carVariant);
            return true;
        }
    }
}
