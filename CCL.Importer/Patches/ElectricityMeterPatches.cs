using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CCL.Importer.Components.Simulation.Electric;
using CCL.Importer.Implementations;

using DV.Damage;
using DV.ServicePenalty;
using DV.Simulation.Cars;
using DV.ThingTypes;

using HarmonyLib;

using LocoSim.Implementations;

using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(SimController), "OnLogicCarInitialized")]
    internal static class LogicCarInitializerPatch
    {
        private static void Postfix(SimController? __instance)
        {
            TrainCar? vehicle = __instance?.train;
            SimulatedCarDebtTracker? feeTracker = __instance?.debt;
            if (vehicle != null && feeTracker != null)
            {
                ElectricityMeter.AddNewTracker(vehicle, feeTracker);
            }
        }
    }
        
    [HarmonyPatch(typeof(SimulatedCarDebtTracker))]
    internal class ElectricityMeterPatches
    {
        [HarmonyPatch("InitializeDebtComponents")]
        [HarmonyPrefix]
        private static void InitializeDebtComponentsPrefix(SimulatedCarDebtTracker? __instance, out bool __state)
        {
            __state = false;
            DamageController? damageController = __instance?.dmgController;
            Dictionary<ResourceType, List<ResourceContainer>>? resourceKeysToResourceContainers = __instance?.resourceToResourceContainers;
            if (damageController == null || resourceKeysToResourceContainers == null)
            {
                return;
            }
            var unit = TrainCar.Resolve(damageController.gameObject);
            if (unit == null || unit.gameObject.GetComponentInChildren<ElectricityMeterDefinitionInternal>() == null)
            { 
                return; 
            }
            
            __state = true;
            bool hasElectricChargeContainer = false;
            foreach (KeyValuePair<ResourceType, List<ResourceContainer>> trackedResource in resourceKeysToResourceContainers)
            {
                if (trackedResource.Key == ResourceType.ElectricCharge)
                {
                    hasElectricChargeContainer = true;
                    break;
                }
            }
            if (!hasElectricChargeContainer)
            { 
                resourceKeysToResourceContainers[ResourceType.ElectricCharge] = new(); 
            }
        }

        [HarmonyPatch("InitializeDebtComponents")]
        [HarmonyPostfix]
        private static void InitializeDebtComponentsPostfix(SimulatedCarDebtTracker? __instance, bool __state)
        {
            if (__state && __instance != null)
            {
                Dictionary<ResourceType, List<ResourceContainer>> resourceKeysToResourceContainers = __instance.resourceToResourceContainers;
                ElectricityMeter.initialElectricCharge[__instance] = 0.0f;
                foreach (ResourceContainer electricChargeContainer in resourceKeysToResourceContainers[ResourceType.ElectricCharge])
                { 
                    ElectricityMeter.initialElectricCharge[__instance] += electricChargeContainer.amountReadOut.Value; 
                }
            }
        }

        [HarmonyPatch("UpdateDebtValues")]
        [HarmonyPrefix]
        private static void UpdateDebtValuesPrefix(SimulatedCarDebtTracker? __instance, out float? __state)
        {
            __state = null;
            if (__instance == null || !ElectricityMeter.initialElectricCharge.TryGetValue(__instance, out float initialCharge))
            { 
                return; 
            }

            // Reset the start and snapshot values of the debt component to vanilla settings for consistency.
            // The snapshot value might become negative temporarily, but doing it is safe as UpdateDebtValues()
            // doesn't deal with snapshots.
            foreach (DebtComponent currentFee in __instance.GetTrackedDebts())
            {
                if (currentFee.Type == ResourceType.ElectricCharge && ElectricityMeter.feeTrackers.ContainsKey(__instance))
                { 
                    if (currentFee.HasSnapshot)
                    { 
                        __state = currentFee.StartValue - currentFee.SnapshotValue; 
                    }
                    currentFee.UpdateStartValue(initialCharge);
                    if (__state != null)
                    { 
                        currentFee.SetSnapshot(currentFee.StartValue - (float) __state); 
                    }
                    break;
                }
            }
        }

        [HarmonyPatch("UpdateDebtValues")]
        [HarmonyPostfix]
        private static void UpdateDebtValuesPostfix(SimulatedCarDebtTracker? __instance, float? __state)
        {
            if (__instance == null || !ElectricityMeter.initialElectricCharge.TryGetValue(__instance, out float initialCharge))
            { 
                return; 
            }
            foreach (DebtComponent currentFee in __instance.GetTrackedDebts())
            {
                if (currentFee.Type == ResourceType.ElectricCharge && ElectricityMeter.feeTrackers.TryGetValue(__instance, out ElectricityMeter meter))
                { 
                    float newEndValue = currentFee.EndValue - Mathf.Max((float) meter.energyConsumed, 0.0f);
                    float minimum = (__state != null) ? Mathf.Min(newEndValue, currentFee.SnapshotValue) : newEndValue;
                    if (minimum >= 0.0f)
                    { 
                        currentFee.UpdateEndValue(newEndValue); 
                    }
                    else
                    {
                        currentFee.UpdateEndValue(0.0f);
                        currentFee.UpdateStartValue(initialCharge - minimum);
                        if (__state != null)
                        { 
                            currentFee.SetSnapshot(currentFee.StartValue - (float) __state); 
                        }
                    }
                    break;
                }
            }
        }

        [HarmonyPatch("ResetState")]
        [HarmonyPostfix]
        private static void ResetStatePostfix(SimulatedCarDebtTracker? __instance)
        {
            if (__instance != null && ElectricityMeter.feeTrackers.TryGetValue(__instance, out ElectricityMeter meter))
            {
                meter.Reset();
            }
        }
    }
}
