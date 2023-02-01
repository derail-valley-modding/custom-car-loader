using DV.Logic.Job;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DVCustomCarLoader.Patches
{
    public static class JobPaymentPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StationProceduralJobGenerator), "ExtractPaymentCalculationData")]
        public static bool ExtractPaymentCalculationData(object carTypesPerCargoData, ref PaymentCalculationData __result)
        {
            var cars = new Dictionary<TrainCarType, int>();
            var cargos = new Dictionary<CargoType, float>();

            foreach (dynamic carData in carTypesPerCargoData as IEnumerable)
            {
                List<TrainCarType> carTypes = carData.carTypes;
                CargoType cargoType = carData.cargoType;
                float totalCargoAmount = carData.totalCargoAmount;

                if (!cargos.TryGetValue(cargoType, out float existAmount))
                {
                    existAmount = 0;
                }
                cargos[cargoType] = existAmount + totalCargoAmount;

                foreach (var carType in carTypes)
                {
                    if (!cars.TryGetValue(carType, out int count))
                    {
                        count = 0;
                    }

                    cars[carType] = count + 1;
                }
            }

            __result = new ExtendedPaymentData(cars, cargos);
            return false;
        }

        private static readonly FieldInfo _cargoDataField = AccessTools.Field(typeof(PaymentCalculationData), nameof(PaymentCalculationData.cargoData));
        private static readonly FieldInfo _floatCargoDataField = AccessTools.Field(typeof(ExtendedPaymentData), nameof(ExtendedPaymentData.floatCargoData));

        private static void CalculateCargoPayment(PaymentCalculationData paymentData, ref float cargoMass, ref float damagePrice, ref float environmentPrice)
        {
            IEnumerable<KeyValuePair<CargoType, float>> kvpEnum;

            if (paymentData is ExtendedPaymentData eData)
            {
                kvpEnum = eData.floatCargoData;
            }
            else
            {
                kvpEnum = paymentData.cargoData.Select(kvp => new KeyValuePair<CargoType, float>(kvp.Key, kvp.Value));
            }

            foreach ((CargoType cargoType, float amount) in kvpEnum)
            {
                cargoMass += CargoTypes.GetCargoUnitMass(cargoType) * amount;
                damagePrice += ResourceTypes.GetFullDamagePriceForCargo(cargoType) * amount;
                environmentPrice += ResourceTypes.GetFullEnvironmentDamagePriceForCargo(cargoType) * amount;
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(JobPaymentCalculator), nameof(JobPaymentCalculator.CalculateJobPayment))]
        public static IEnumerable<CodeInstruction> TranspileCalculateJobPayment(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            bool discardingOps = false;

            // Locals:
            // V_4  cargoMass
            // V_5  damagePrice
            // V_6  environmentPrice

            foreach (var instruction in instructions)
            {
                if (instruction.Is(OpCodes.Ldfld, _cargoDataField))
                {
                    // paymentCalculationData on the stack

                    // push ref cargoMass
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 4);

                    // push ref damagePrice
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 5);

                    // push ref environmentPrice
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 6);

                    // call CalculateCargoPayment
                    yield return CodeInstruction.Call(typeof(JobPaymentPatches), nameof(JobPaymentPatches.CalculateCargoPayment));

                    discardingOps = true;
                }
                else if (instruction.opcode == OpCodes.Ldarg_1)
                {
                    // first op after the foreach loop replaced by CalculateCargoPayment
                    yield return instruction;
                    discardingOps = false;
                }
                else
                {
                    if (!discardingOps)
                    {
                        yield return instruction;
                    }
                }
            }
        }
    }

    public class ExtendedPaymentData : PaymentCalculationData
    {
        public Dictionary<CargoType, float> floatCargoData;

        public ExtendedPaymentData(Dictionary<TrainCarType, int> carsData, Dictionary<CargoType, float> cargoData) :
            base(carsData, cargoData.ToDictionary(kvp => kvp.Key, kvp => (int)kvp.Value))
        {
            floatCargoData = cargoData;
        }
    }
}
