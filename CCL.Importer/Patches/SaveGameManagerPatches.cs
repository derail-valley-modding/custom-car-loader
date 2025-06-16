using CCL.Importer.WorkTrains;
using HarmonyLib;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(SaveGameManager))]
    internal class SaveGameManagerPatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(SaveGameManager.DoSaveIO))]
        private static void DoSaveIOPrefix(SaveGameData data)
        {
            WorkTrainPurchaseHandler.Save(data);
            CarManager.SaveMapping(data);
        }

        [HarmonyPostfix, HarmonyPatch(nameof(SaveGameManager.FindStartGameData))]
        private static void FindStartGameDataPostfix(SaveGameManager __instance)
        {
            WorkTrainPurchaseHandler.Load(__instance.data);
            CarManager.LoadMapping(__instance.data);
            CarManager.ApplyMapping();
        }
    }
}
