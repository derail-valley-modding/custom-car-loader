using CCL_GameScripts;
using DV.Simulation.Brake;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DVCustomCarLoader.LocoComponents.Utility
{
    [HarmonyPatch(typeof(Brakeset))]
    public static class BrakeSetPatches
    {
        public static bool HasBrakePipeControls(bool hasCompressor, BrakeSystem brakeSystem)
        {
            if (hasCompressor) return true;

            TrainCar car = brakeSystem.GetComponent<TrainCar>();
            return 
                CarTypeInjector.IsInCustomRange(car.carType) &&
                CarTypeInjector.CustomCarByType(car.carType).LocoType == LocoParamsType.Caboose;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Brakeset.Update))]
        public static IEnumerable<CodeInstruction> TranspileUpdate(IEnumerable<CodeInstruction> instructions)
        {
            const int brakeSystemLocation = 17; // V_17: BrakeSystem brakeSystem

            // for (BrakeSystem brakeSystem in brakeSet.cars)

            // replace if(brakesystem.hasCompressor) with hasCompressor || IsCustomCaboose

            bool firstBrFound = false;
            foreach (var instruction in instructions)
            {
                if (!firstBrFound && (instruction.opcode == OpCodes.Brfalse))
                {
                    firstBrFound = true;

                    // stack 1: brakeSystem.hasCompressor
                    // push brakeSystem
                    yield return new CodeInstruction(OpCodes.Ldloc_S, brakeSystemLocation);
                    yield return CodeInstruction.Call("BrakeSetPatches:HasBrakePipeControls");
                }

                yield return instruction;
            }
        }
    }
}