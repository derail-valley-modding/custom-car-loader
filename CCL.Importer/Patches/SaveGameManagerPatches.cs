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
            CarPurchaseHandler.Save(data);
        }

        [HarmonyPostfix, HarmonyPatch(nameof(SaveGameManager.FindStartGameData))]
        private static void FindStartGameDataPostfix(SaveGameManager __instance)
        {
            if (__instance.data == null) return;

            CarPurchaseHandler.LoadFromSave(__instance.data);
        }
    }
}
