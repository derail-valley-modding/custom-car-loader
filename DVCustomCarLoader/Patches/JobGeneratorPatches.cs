using DV.Logic.Job;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DVCustomCarLoader.Patches
{
    [HarmonyPatch(typeof(StationProceduralJobGenerator))]
    public static class JobGeneratorPatches
    {
        private static readonly MethodInfo _addCarTypeMethod = AccessTools.Method(typeof(List<TrainCarType>), "Add");
        private static readonly Type _carDataType;
        private static readonly Type _carDataListType;
        private static readonly FieldInfo _carData_carTypesField;

        private static readonly MethodInfo _list_getCountMethod = AccessTools.Method(typeof(List<TrainCarType>), "get_Count");

        static JobGeneratorPatches()
        {
            var jobGenType = AccessTools.TypeByName("StationProceduralJobGenerator");
            _carDataType = jobGenType.GetNestedType("CarTypesPerCargoTypeData", BindingFlags.NonPublic);

            _carDataListType = typeof(List<>).MakeGenericType(_carDataType);
            _carData_carTypesField = AccessTools.Field(_carDataType, "carTypes");
        }

        private static float GetCarCapacity(TrainCarType carType)
        {
            if (CarTypeInjector.TryGetCustomCarByType(carType, out CustomCar car))
            {
                return car.CargoCapacity;
            }
            return 1;
        }

        [HarmonyTranspiler]
        [HarmonyPatch("GenerateBaseCargoTrainData")]
        public static IEnumerable<CodeInstruction> GenerateBaseCargoTrainDataTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var carTypeLocal = generator.DeclareLocal(typeof(TrainCarType));

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(_addCarTypeMethod))
                {
                    // store selected car type to local
                    yield return new CodeInstruction(OpCodes.Dup);
                    yield return new CodeInstruction(OpCodes.Stloc_S, carTypeLocal.LocalIndex);

                    yield return instruction;
                }
                else if (instruction.Is(OpCodes.Ldc_R4, 1f))
                {
                    // replace "totalCargoAmount += 1f" with "totalCargoAmount += car.cargoCapacity"
                    // push carType (arg 1)
                    yield return new CodeInstruction(OpCodes.Ldloc_S, carTypeLocal.LocalIndex);

                    // pop carType, call GetCarCapacity, push capacity
                    yield return CodeInstruction.Call(typeof(JobGeneratorPatches), nameof(GetCarCapacity));
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch("GenerateOutChainJob")]
        public static IEnumerable<CodeInstruction> GenerateOutChainJobTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var carTypeLocal = generator.DeclareLocal(typeof(TrainCarType));
            bool firstLoad = true;

            // "ldc.r4 1" only used in targeted loop
            foreach (var instruction in instructions)
            {
                // inside the loop over the generated CarTypesPerCargoType objects
                // we're combining them all into master cargo type & amount lists

                // V_3      List<CarTypesPerCargoTypeData> carData
                // V_30     List<float> cargoAmountPerCar
                // V_43     int i
                // V_44     int totalCargoAmount
                // V_46     int j

                // for (int i = 0; i < carData.Count; i++)
                // for (int j = 0; j < carData[i].carTypes.Count; j++)

                if (instruction.Is(OpCodes.Ldc_R4, 1f))
                {
                    if (firstLoad)
                    {
                        // cargoAmountPerCar.Add(1f) => cargoAmountPerCar.Add(capacity)
                        // IL_0495: push 1f
                        // stack holds reference to cargoAmountPerCar

                        // push carData (V_3)
                        yield return new CodeInstruction(OpCodes.Ldloc_3);

                        // push i (V_43)
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 43);

                        // call List<CarTypesPerCargoTypeData>.get_Item(int)
                        yield return CodeInstruction.Call(_carDataListType, "get_Item");

                        // load field CarTypesPerCargoTypeData.carTypes
                        yield return new CodeInstruction(OpCodes.Ldfld, _carData_carTypesField);

                        // push j (V_46)
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 46);

                        // call List<TrainCarType>.get_Item(int)
                        yield return CodeInstruction.Call(typeof(List<TrainCarType>), "get_Item");

                        // save car type
                        yield return new CodeInstruction(OpCodes.Dup);
                        yield return new CodeInstruction(OpCodes.Stloc_S, carTypeLocal.LocalIndex);

                        // call JobGeneratorPatches.GetCarCapacity(TrainCarType)
                        yield return CodeInstruction.Call(typeof(JobGeneratorPatches), nameof(GetCarCapacity));

                        firstLoad = false;
                    }
                    else
                    {
                        // totalCargoAmount -= 1f => totalCargoAmount -= capacity
                        // IL_04A1: push 1f
                        // stack holds reference to totalCargoAmount

                        // push carType
                        yield return new CodeInstruction(OpCodes.Ldloc_S, carTypeLocal.LocalIndex);

                        // call JobGeneratorPatches.GetCarCapacity(TrainCarType)
                        yield return CodeInstruction.Call(typeof(JobGeneratorPatches), nameof(GetCarCapacity));
                    }
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
