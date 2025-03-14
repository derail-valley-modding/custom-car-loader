using DV.RenderTextureSystem.BookletRender;
using DV.ThingTypes;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(TaskTemplatePaper))]
    internal class TaskTemplatePaperPatches
    {
        private static FieldInfo? s_iconField;
        private static FieldInfo IconField => Extensions.GetCached(ref s_iconField, () =>
            typeof(CargoType_v2).GetField(nameof(CargoType_v2.icon)));

        //[HarmonyTranspiler, HarmonyPatch(nameof(TaskTemplatePaper.FillInData))]
        //private static IEnumerable<CodeInstruction> FillInDataTranspiler(IEnumerable<CodeInstruction> instructions)
        //{
        //    foreach (var instruction in instructions)
        //    {
        //        if (instruction.LoadsField(IconField))
        //        {
        //            yield return CodeInstruction.LoadField();
        //        }
        //        else
        //        {
        //            yield return instruction;
        //        }
        //    }
        //}
    }
}
