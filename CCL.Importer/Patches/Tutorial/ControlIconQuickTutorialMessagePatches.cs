using CCL.Importer.Implementations.Controls;
using DV.Tutorial.QT;
using HarmonyLib;
using UnityEngine;

namespace CCL.Importer.Patches.Tutorial
{
    [HarmonyPatch(typeof(ControlIconQuickTutorialMessage))]
    internal class ControlIconQuickTutorialMessagePatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(ControlIconQuickTutorialMessage.WithSprite))]
        private static bool WithSpritePrefix(ControlIconQuickTutorialMessage __instance, Behaviour target, QTSemantic semantic,
            ref ControlIconQuickTutorialMessage __result)
        {
            if (semantic == QTSemantic.Look || semantic == QTSemantic.Monitor || target is not ScrewBase) return true;

            __instance.spriteIndex = 6;
            __result = __instance;
            return false;
        }
    }
}
