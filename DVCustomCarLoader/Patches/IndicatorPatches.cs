using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace DVCustomCarLoader.Patches
{
    [HarmonyPatch]
    public static class IndicatorPatches
    {
        public static float AdjustIndicatorEmissionLevel(float normalized, float min, float max)
        {
            return Mathf.Lerp(min, max, normalized);
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(IndicatorEmission), "OnValueSet")]
        public static IEnumerable<CodeInstruction> IndicatorEmission_OnValueSet(IEnumerable<CodeInstruction> instructions)
        {
            // insert call to AdjustIndicatorEmissionLevel directly before first stloc.0
            bool patched = false;

            foreach (var instruction in instructions)
            {
                if (!patched && (instruction.opcode == OpCodes.Stloc_0))
                {
                    patched = true;

                    // push IndicatorEmission.minEmission to stack
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return CodeInstruction.LoadField(typeof(IndicatorEmission), nameof(IndicatorEmission.minEmission));

                    // push IndicatorEmission.maxEmission to stack
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return CodeInstruction.LoadField(typeof(IndicatorEmission), nameof(IndicatorEmission.maxEmission));

                    // call AdjustIndicatorEmissionLevel()
                    yield return CodeInstruction.Call("IndicatorPatches:AdjustIndicatorEmissionLevel");
                }

                yield return instruction;
            }
        }
    }
}
