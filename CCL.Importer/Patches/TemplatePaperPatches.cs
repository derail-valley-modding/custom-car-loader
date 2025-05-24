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
            if (!CatalogGenerator.PageInfos.TryGetValue(__instance.carLivery, out var layout)) return;

            // ?????????????
            foreach (var item in __instance.GetComponentsInChildren<Localize>())
            {
                item.UpdateLocalization();
            }

            // Reprocess the comms radio and garage icons.
            if (layout.SummonableByRemote)
            {
                __instance.summon.icon.gameObject.SetActive(true);
                __instance.summon.price.gameObject.SetActive(true);
                __instance.summon.price.SetTextAndUpdate(CatalogGenerator.FormatPrice(layout.SummonPrice));
            }

            if (layout.UnlockedByGarage)
            {
                __instance.garage.icon.gameObject.SetActive(true);
                __instance.garage.price.gameObject.SetActive(true);
                __instance.garage.price.SetTextAndUpdate(CatalogGenerator.FormatPrice(layout.GaragePrice));
            }
        }
    }
}
