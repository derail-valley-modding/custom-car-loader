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
    public static class StationProceduralJobGeneratorPatches
    {
        private static readonly MethodInfo _addCarTypeMethod = AccessTools.Method(typeof(List<TrainCarType>), "Add");
        private static readonly Type _carDataType;
        private static readonly Type _carDataListType;
        private static readonly FieldInfo _carData_carTypesField;

        private static readonly MethodInfo _list_getCountMethod = AccessTools.Method(typeof(List<TrainCarType>), "get_Count");

        static StationProceduralJobGeneratorPatches()
        {
            var jobGenType = typeof(StationProceduralJobGenerator);
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
                    yield return CodeInstruction.Call(typeof(StationProceduralJobGeneratorPatches), nameof(GetCarCapacity));
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

                        // call GetCarCapacity(TrainCarType)
                        yield return CodeInstruction.Call(typeof(StationProceduralJobGeneratorPatches), nameof(GetCarCapacity));

                        firstLoad = false;
                    }
                    else
                    {
                        // totalCargoAmount -= 1f => totalCargoAmount -= capacity
                        // IL_04A1: push 1f
                        // stack holds reference to totalCargoAmount

                        // push carType
                        yield return new CodeInstruction(OpCodes.Ldloc_S, carTypeLocal.LocalIndex);

                        // call GetCarCapacity(TrainCarType)
                        yield return CodeInstruction.Call(typeof(StationProceduralJobGeneratorPatches), nameof(GetCarCapacity));
                    }
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

    [HarmonyPatch(typeof(JobsGenerator))]
    public static class JobsGeneratorPatches
    {
        private static readonly FieldInfo _car_capacityField = AccessTools.Field(typeof(Car), nameof(Car.capacity));
        private static readonly FieldInfo _totalCargoAmountField = AccessTools.Field(typeof(CarsPerCargoType), nameof(CarsPerCargoType.totalCargoAmount));

        private class ShuntingAnonTypeInfo
        {
            // Display Class 1 (outer)
            public readonly Type DC1_Type;
            public readonly FieldInfo DC1_CS8locals1Field;
            public readonly FieldInfo DC1_iField;

            // Display Class 0 (inner)
            public readonly Type DC0_Type;
            public readonly FieldInfo DC0_carsLoadDataField;

            public readonly int DC1_LocalIndex;

            public ShuntingAnonTypeInfo(int methodNumber, string loadDataName, int dc1LocalIndex)
            {
                DC1_Type = typeof(JobsGenerator).GetNestedType($"<>c__DisplayClass{methodNumber}_1", BindingFlags.NonPublic);
                DC1_CS8locals1Field = DC1_Type.GetField("CS$<>8__locals1");
                DC1_iField = DC1_Type.GetField("i");

                DC0_Type = typeof(JobsGenerator).GetNestedType($"<>c__DisplayClass{methodNumber}_0", BindingFlags.NonPublic);
                DC0_carsLoadDataField = DC0_Type.GetField(loadDataName);

                DC1_LocalIndex = dc1LocalIndex;
            }
        }

        private static readonly ShuntingAnonTypeInfo _shuntingLoadTypeInfo = new ShuntingAnonTypeInfo(0, "carsLoadData", 10);
        private static readonly ShuntingAnonTypeInfo _shuntingUnloadTypeInfo = new ShuntingAnonTypeInfo(1, "carsUnloadData", 8);


        [HarmonyTranspiler]
        [HarmonyPatch(nameof(JobsGenerator.CreateTransportJob))]
        public static IEnumerable<CodeInstruction> TranspileCreateTransportJob(IEnumerable<CodeInstruction> instructions)
        {
            // Arguments:
            // A_2  List<Car> cars
            // A_6  List<float> cargoAmountPerCar

            // Locals:
            // V_2  int i

            Label? branchDest = null;

            foreach (var instruction in instructions)
            {

                // bge.un.s is only used in the comparison "if (cars[i].capacity < cargoAmountPerCar[i])"
                // the instructions within the if-then body throw an exception
                // replace that with assigning "cargoAmountPerCar[i] = cars[i].capacity"
                if (instruction.opcode == OpCodes.Bge_Un_S)
                {
                    branchDest = (Label)instruction.operand;
                    yield return instruction;

                    // push cargoAmountPerCar
                    yield return new CodeInstruction(OpCodes.Ldarg_S, 6);

                    // push i
                    yield return new CodeInstruction(OpCodes.Ldloc_2);

                    // push cars[i].capacity
                    {
                        // push cars
                        yield return new CodeInstruction(OpCodes.Ldarg_2);

                        // push i
                        yield return new CodeInstruction(OpCodes.Ldloc_2);

                        // call List<Car>.get_Item(int)
                        yield return CodeInstruction.Call(typeof(List<Car>), "get_Item");

                        // ldfld Car.capacity
                        yield return new CodeInstruction(OpCodes.Ldfld, _car_capacityField);
                    }

                    // call List<float>.set_Item(int, float)
                    yield return CodeInstruction.Call(typeof(List<float>), "set_Item");
                }
                else if (branchDest.HasValue && instruction.labels.Contains(branchDest.Value))
                {
                    // end of if-then body
                    branchDest = null;
                    yield return instruction;
                }
                else if (!branchDest.HasValue)
                {
                    yield return instruction;
                }
            }
        }

        

        private static void RecalculateTotalCargoAmount(CarsPerCargoType carsLoadData)
        {
            float capacity = carsLoadData.cars.Sum(c => c.capacity);
            _totalCargoAmountField.SetValue(carsLoadData, capacity);
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(JobsGenerator.CreateShuntingLoadJob))]
        public static IEnumerable<CodeInstruction> TranspileCreateShuntingLoadJob(IEnumerable<CodeInstruction> instructions)
        {
            return TranspileCreateShuntingJob(instructions, _shuntingLoadTypeInfo);
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(JobsGenerator.CreateShuntingUnloadJob))]
        public static IEnumerable<CodeInstruction> TranspileCreateShuntingUnloadJob(IEnumerable<CodeInstruction> instructions)
        {
            return TranspileCreateShuntingJob(instructions, _shuntingUnloadTypeInfo);
        }

        private static IEnumerable<CodeInstruction> TranspileCreateShuntingJob(IEnumerable<CodeInstruction> instructions, ShuntingAnonTypeInfo typeInfo)
        {
            // private class <>c__DisplayClassN_1 {
            //     public int i;
            //     public <>c__DisplayClassN_0 CS$<>8__locals1;
            // }

            // private class <>c__DisplayClassN_0 {
            //     public List<CarsPerCargoType> carsLoadData;
            // }

            // Locals
            // V_N      <>c__DisplayClassN_1

            Label? branchDest = null;

            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Bge_Un)
                {
                    // recalculate totalCargoAmount
                    yield return instruction;
                    branchDest = (Label)instruction.operand;

                    // push V_N.CS$<>8__locals1.carsLoadData[i]
                    // push V_N
                    yield return new CodeInstruction(OpCodes.Ldloc_S, typeInfo.DC1_LocalIndex);

                    // ldlfd V_N.CS$<>8__locals1
                    yield return new CodeInstruction(OpCodes.Ldfld, typeInfo.DC1_CS8locals1Field);

                    // ldfld V_N.CS$<>8__locals1.carsLoadData
                    yield return new CodeInstruction(OpCodes.Ldfld, typeInfo.DC0_carsLoadDataField);

                    // push V_N
                    yield return new CodeInstruction(OpCodes.Ldloc_S, typeInfo.DC1_LocalIndex);

                    // ldfld V_N.i
                    yield return new CodeInstruction(OpCodes.Ldfld, typeInfo.DC1_iField);

                    // call List<CarsPerCargoType>.get_Item(int)
                    yield return CodeInstruction.Call(typeof(List<CarsPerCargoType>), "get_Item");

                    // call RecalculateTotalCargoAmount
                    yield return CodeInstruction.Call(typeof(JobsGeneratorPatches), nameof(JobsGeneratorPatches.RecalculateTotalCargoAmount));
                }
                else if (branchDest.HasValue && instruction.labels.Contains(branchDest.Value))
                {
                    // end of if-then body
                    branchDest = null;
                    yield return instruction;
                }
                else if (!branchDest.HasValue)
                {
                    yield return instruction;
                }
            }
        }
    }
}
