using CCL.Importer.Types;
using DV;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using HarmonyLib;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(JobPaymentCalculator))]
    internal class JobPaymentCalculatorPatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(JobPaymentCalculator.CalculateJobPayment))]
        private static bool CalculateJobPaymentPrefix(JobType jobType, float distanceInMeters, PaymentCalculationData paymentCalculationData, ref float __result)
        {
            if (paymentCalculationData is not ExtendedPaymentData data) return true;

            // Car value is the same as the base.
            int carCount = 0;
            float carMass = 0;
            float carPrice = 0;

            foreach (var car in paymentCalculationData.carsData)
            {
                int count = car.Value;
                var parentType = car.Key.parentType;
                carMass += parentType.mass * count;
                carPrice += parentType.damage.bodyPrice * count;
                carCount += count;
            }

            int cargoCount = 0;
            float cargoMass = 0;
            float cargoValue = 0;
            float cargoEnvPrice = 0;
            float cargoSensitivityMod = 0;

            // Original cargo calculation.
            //foreach (var cargo in paymentCalculationData.cargoData)
            //{
            //    var v2 = cargo.Key.ToV2();
            //    if (v2 == null) continue;

            //    int count = cargo.Value;
            //    cargoMass += v2.massPerUnit * count;
            //    cargoValue += v2.fullDamagePrice * count;
            //    cargoEnvPrice += v2.environmentDamagePrice * count;
            //    cargoSensitivityMod += v2.sensitivityPaymentModifier * count;
            //    cargoCount += count;
            //}

            foreach (var extended in data.ExtendedData)
            {
                var v2 = extended.Cargo.ToV2();
                if (v2 == null) continue;

                float mult = 1.0f;
                cargoCount += extended.Count;

                if (extended.CarType is CCL_CarType car)
                {
                    if (!car.CargoAmounts.TryGetValue(v2.id, out mult))
                    {
                        CCLPlugin.Error($"Could not get cargo '{v2.id}' amount for car type {car.id}! Defaulting to 1.");
                        mult = 1;
                    }
                }

                mult *= extended.Count;

                cargoMass += v2.massPerUnit * mult;
                cargoValue += v2.fullDamagePrice * mult;
                cargoEnvPrice += v2.environmentDamagePrice * mult;
                cargoSensitivityMod += v2.sensitivityPaymentModifier * extended.Count;
            }

            float distance = distanceInMeters * JobPaymentCalculator.DISTANCE_MULTIPLIER;
            float payment = carMass * JobPaymentCalculator.CAR_MASS_MULTIPLIER +
                carPrice * JobPaymentCalculator.CAR_VALUE_MULTIPLIER +
                cargoMass * JobPaymentCalculator.CARGO_MASS_MULTIPLIER +
                cargoValue * JobPaymentCalculator.CARGO_VALUE_MULTIPLIER +
                cargoEnvPrice * JobPaymentCalculator.CARGO_ENVIRONMENT_DAMAGE_VALUE_MULTIPLIER;
            float boringnessFactor = JobPaymentCalculator.GetBoringnessFactor(jobType);
            float falloffFactor = JobPaymentCalculator.GetFalloffFactor(carCount);
            int diff = Mathf.Max(carCount - cargoCount, 0);
            float sensitivity = (cargoSensitivityMod + diff) / (cargoCount + diff);

            __result = Mathf.Round((JobPaymentCalculator.BASE_PAYMENT + payment * distance * boringnessFactor * falloffFactor * sensitivity) *
                Globals.G.GameParams.JobPaymentModifier);

            return false;
        }
    }
}
