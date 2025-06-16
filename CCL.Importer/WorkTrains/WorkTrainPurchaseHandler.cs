using CCL.Importer.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCL.Importer.WorkTrains
{
    internal class WorkTrainPurchaseHandler
    {
        public static readonly List<CCL_CarVariant> WorkTrainLiveries = new();
        public static Action<CCL_CarVariant>? LiveryUnlocked;

        private static readonly List<CCL_CarVariant> s_unlocked = new();
        private static readonly List<string> s_ids = new();
        private static readonly HashSet<string> s_alreadySummoned = new();

        public static List<CCL_CarVariant> UnlockedLiveries => s_unlocked.ToList();
        public static List<CCL_CarVariant> LockedLiveries => WorkTrainLiveries.Except(s_unlocked).ToList();

        public static bool Unlock(CCL_CarVariant livery)
        {
            if (s_unlocked.Contains(livery) || !WorkTrainLiveries.Contains(livery)) return false;

            var vehicles = CarSpawner.Instance.vehiclesWithoutGarage.ToList();
            vehicles.Add(livery);
            CarSpawner.Instance.vehiclesWithoutGarage = vehicles.ToArray();
            s_unlocked.Add(livery);
            s_ids.Add(livery.id);
            LiveryUnlocked?.Invoke(livery);
            return true;
        }

        public static bool SetAsSummoned(CCL_CarVariant livery)
        {
            if (!WorkTrainLiveries.Contains(livery)) return false;

            return SetAsSummoned(livery.id);
        }

        private static bool SetAsSummoned(string id)
        {
            if (s_alreadySummoned.Contains(id)) return false;

            s_alreadySummoned.Add(id);
            return true;
        }

        public static bool HasBeenSummonedBefore(CCL_CarVariant livery)
        {
            return s_alreadySummoned.Contains(livery.id);
        }

        public static void Load(SaveGameData data)
        {
            s_unlocked.Clear();
            s_ids.Clear();
            s_alreadySummoned.Clear();

            if (data == null) return;

            var unlocked = data.GetStringArray(SaveConstants.UNLOCKED_WORK_TRAINS);

            if (unlocked == null) return;

            foreach (var id in unlocked)
            {
                if (WorkTrainLiveries.TryFind(x => x.id == id, out var livery))
                {
                    Unlock(livery);
                }
                else
                {
                    s_ids.Add(id);
                }
            }

            var summoned = data.GetStringArray(SaveConstants.SUMMONED_WORK_TRAINS);

            if (summoned == null) return;

            foreach (var id in summoned)
            {
                SetAsSummoned(id);
            }
        }

        public static void Save(SaveGameData data)
        {
            data.SetStringArray(SaveConstants.UNLOCKED_WORK_TRAINS, s_ids.Distinct().ToArray());
            data.SetStringArray(SaveConstants.SUMMONED_WORK_TRAINS, s_alreadySummoned.ToArray());
        }
    }
}
