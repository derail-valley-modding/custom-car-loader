using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CCL_GameScripts;

namespace DVCustomCarLoader.Patches
{
    [HarmonyPatch]
    public static class CouplerPatches
    {
        private static readonly MethodInfo _transformFindMethod = AccessTools.Method(typeof(Transform), nameof(Transform.Find));
        private static readonly string[] _bogieNames = { CarPartNames.BOGIE_FRONT, CarPartNames.BOGIE_REAR };

        private static Transform FindCouplerTransform(Transform carRoot, string name)
        {
            Transform result = carRoot.Find(name);

            if (!result)
            {
                foreach (string bogieName in _bogieNames)
                {
                    var bogie = carRoot.Find(bogieName);
                    result = bogie ? bogie.Find(name) : null;

                    if (result) break;
                }
            }

            return result;
        }

        [HarmonyPatch(typeof(TrainCar), "SetupCouplers")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> TranspileSetupCouplers(IEnumerable<CodeInstruction> instructions)
        {
            // replace Transform.Find with FindRecursive

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(_transformFindMethod))
                {
                    yield return CodeInstruction.Call(typeof(CouplerPatches), nameof(CouplerPatches.FindCouplerTransform));
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        private static readonly FieldInfo _couplerTrainField = AccessTools.Field(typeof(Coupler), nameof(Coupler.train));

        [HarmonyPatch(typeof(Coupler), "CreateRigidJoint")]
        [HarmonyPatch(typeof(Coupler), "CreateSpringyJoint")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> TranspileCreateJoint(IEnumerable<CodeInstruction> instructions)
        {
            // remove second load Coupler::train call, so that we get the coupler's rigidbody and not the train's
            int getTrainCounter = 0;

            foreach (var instruction in instructions)
            {
                if (instruction.LoadsField(_couplerTrainField))
                {
                    getTrainCounter++;

                    if (getTrainCounter == 2)
                    {
                        continue;
                    }
                }

                yield return instruction;
            }
        }
    }
}
