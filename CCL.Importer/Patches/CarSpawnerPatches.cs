using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(CarSpawner))]
    public static class CarSpawner_GetFromPool_Patch
    {
        private static TrainCar GetDisabledTrainCar(GameObject prefab)
        {
            return prefab.GetComponentsInChildren<TrainCar>(true).FirstOrDefault();
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(CarSpawner.GetFromPool))]
        public static IEnumerable<CodeInstruction> TranspileGetFromPool(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo getcomponentMethod = AccessTools.Method(typeof(GameObject), nameof(GameObject.GetComponent), generics: new[] { typeof(TrainCar) });

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(getcomponentMethod))
                {
                    yield return CodeInstruction.Call(typeof(CarSpawner_GetFromPool_Patch), nameof(GetDisabledTrainCar));
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

    [HarmonyPatch(typeof(CarSpawner))]
    public static class CarSpawner_SpawnCar_Patch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(CarSpawner), nameof(CarSpawner.SpawnCar));
            yield return AccessTools.Method(typeof(CarSpawner), nameof(CarSpawner.SpawnLoadedCar));
        }

        private static GameObject EnableCar(GameObject carObj)
        {
            if (!carObj.activeSelf)
            {
                carObj.SetActive(true);
            }
            return carObj;
        }

        [HarmonyTranspiler]
        [HarmonyPatch]
        public static IEnumerable<CodeInstruction> TranspileSpawnCar(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo getComponentInChildren = AccessTools.Method(typeof(GameObject), nameof(GameObject.GetComponentInChildren), generics: new[] { typeof(TrainCar) });

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(getComponentInChildren))
                {
                    yield return CodeInstruction.Call(typeof(CarSpawner_SpawnCar_Patch), nameof(EnableCar));
                }
                yield return instruction;
            }
        }
    }
}
