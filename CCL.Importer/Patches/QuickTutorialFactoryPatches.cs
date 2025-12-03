using CCL.Importer.Types;
using DV.Tutorial.QT;
using HarmonyLib;

using static DV.Tutorial.QT.QuickTutorialFactory;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(QuickTutorialFactory))]
    internal class QuickTutorialFactoryPatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(QuickTutorialFactory.PrepareFor))]
        private static bool PrepareForPrefix(TrainCar loco, ref QuickTutorial __result)
        {
            if (loco == null) return true;

            if (loco.carLivery.parentType is not CCL_CarType ccl) return true;

            switch (ccl.TutorialType)
            {
                case CCL.Types.CustomCarType.TutorialTypeEnum.Diesel:
                    __result = DieselEngineTutorial(new TrainTutorialConstructor(loco, true), loco);
                    return false;
                case CCL.Types.CustomCarType.TutorialTypeEnum.Steam:
                    __result = SteamEngineTutorial(new TrainTutorialConstructor(loco, true), loco);
                    return false;
                case CCL.Types.CustomCarType.TutorialTypeEnum.Custom:
                    if (ccl.Tutorial == null)
                    {
                        CCLPlugin.Error("Custom tutorial requested but not provided, using default behaviour.");
                        goto default;
                    }

                    var c = new TrainTutorialConstructor(loco, true);

                    __result = TutorialGenerator.CustomTutorialImplementations.TryGetValue(ccl.id, out var function)
                        ? function(c, loco)
                        : TutorialGenerator.Generate(c, loco, ccl.Tutorial);
                    return false;
                default:
                    return true;
            }
        }
    }
}
