using CCL.Importer.Types;
using DV.Localization;
using DV.RenderTextureSystem.BookletRender;
using HarmonyLib;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(VehicleCatalogPageTemplatePaper))]
    internal class VehicleCatalogPageTemplatePaperPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(VehicleCatalogPageTemplatePaper.FillInData))]
        private static void FillInDataPostfix(VehicleCatalogPageTemplatePaper __instance)
        {
            // No need to do anything with the original pages.
            if (__instance.carLivery is not CCL_CarVariant livery || !CatalogGenerator.PageInfos.TryGetValue(livery, out _)) return;

            // ?????????????
            foreach (var item in __instance.GetComponentsInChildren<Localize>())
            {
                item.UpdateLocalization();
            }

            // Reprocess the comms radio and garage icons.
            if (livery.UnlockableAsWorkTrain)
            {
                __instance.summon.icon.gameObject.SetActive(true);
                __instance.summon.price.gameObject.SetActive(true);
                __instance.summon.price.SetTextAndUpdate(CatalogGenerator.FormatPrice(livery.SummonPrice));

                __instance.garage.icon.gameObject.SetActive(true);
                __instance.garage.price.gameObject.SetActive(true);
                __instance.garage.price.SetTextAndUpdate(CatalogGenerator.FormatPrice(livery.UnlockPrice));
            }
        }
    }
}
