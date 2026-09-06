using System.Collections.Generic;
using System.Collections.ObjectModel;

using CCL.Importer.Components.Simulation.Electric;
using CCL.Types.Components.Simulation.Electric;

using DV.Damage;
using DV.JObjectExtstensions;
using DV.ServicePenalty;
using DV.Simulation.Cars;
using DV.ThingTypes;
using DV.Utils;

using HarmonyLib;

using LocoSim.Implementations;

using Newtonsoft.Json.Linq;

using UnityEngine;

namespace CCL.Importer.Implementations
{
    [HarmonyPatch(typeof(SimulatedCarDebtTracker))]
    public class ElectricityMeter : SimComponent
    {
        [HarmonyPatch(typeof(SimController), "OnLogicCarInitialized")]
        private static class LogicCarInitializer
        {
            public static void Postfix(TrainCar? ___train, SimulatedCarDebtTracker? ___debt)
            {
                if (___train != null && ___debt != null)
                {
                    _newTrackers[___train] = ___debt;
                    if (_carsWithMeters.TryGetValue(___train, out ElectricityMeter meter))
                        meter.TrySetupFeeTracker();
                }
            }
        }
        
        private static readonly Dictionary<TrainCar, ElectricityMeter> _carsWithMeters = new();
        private static readonly Dictionary<TrainCar, SimulatedCarDebtTracker> _newTrackers = new();
        private static readonly Dictionary<SimulatedCarDebtTracker, ElectricityMeter> _feeTrackers = new();
        private static readonly Dictionary<SimulatedCarDebtTracker, float> _initialElectricCharge = new();
        
        private readonly TrainCar? _unit;
        private SimulatedCarDebtTracker? _feeTracker;
        
        private readonly FuseReference? _masterFuse = null;
        private readonly Port _electricChargeConsumed;
        private readonly PortReference _supplyVoltage, _currentDraw;

        private float _energyConsumptionFactor;
        private double _energyConsumed = 0.0;

        public override bool HasSaveData => true;
        
        public ElectricityMeter(ElectricityMeterDefinitionInternal definition) : base(definition.ID)
        {
            _energyConsumptionFactor = definition.electricChargeConsumptionFactor / (1000.0f * 3600.0f);

            if (!string.IsNullOrEmpty(definition.masterControlFuseId))
                _masterFuse = AddFuseReference(definition.masterControlFuseId);
            _electricChargeConsumed = AddPort(definition.electricChargeConsumed);
            _supplyVoltage = AddPortReference(definition.supplyVoltage);
            _currentDraw = AddPortReference(definition.currentDraw);

            _unit = TrainCar.Resolve(definition.gameObject);
            if (_unit == null)
            {
                CCLPlugin.Error("Train car not found, electricity meter disabled");
                _feeTracker = null;
                return;
            }
            if (_carsWithMeters.ContainsKey(_unit))
            {
                CCLPlugin.Error("Another electricity meter present on the car, duplicate meters disabled");
                _feeTracker = null;
                return;
            }
            _carsWithMeters[_unit] = this;
            TrySetupFeeTracker();
            if (gameParams == null)
                _unit.LogicCarInitialized += AdjustEnergyConsumptionFactor;
            else
                AdjustEnergyConsumptionFactor();
            _unit.OnDestroyCar += DisposeFeeTracker;
        }

        private void AdjustEnergyConsumptionFactor()
        { 
            if (_unit != null)
            {
                _unit.LogicCarInitialized -= AdjustEnergyConsumptionFactor;
                _energyConsumptionFactor *= gameParams.ResourceConsumptionModifier;
            }
        }

        private void TrySetupFeeTracker()
        {
            if (_unit == null || !_carsWithMeters.ContainsKey(_unit) || !_newTrackers.ContainsKey(_unit))
                return;
            _feeTracker = _newTrackers[_unit];
            _feeTrackers[_feeTracker] = this;
            _newTrackers.Remove(_unit);
            _feeTracker.UpdateDebtValues();
            CCLPlugin.LogVerbose($"Set up a fee tracker for car {_unit.ID}");
        }

        private void DisposeFeeTracker(TrainCar unit)
        {
            unit.OnDestroyCar -= DisposeFeeTracker;
            if (_feeTracker != null && _feeTrackers.ContainsKey(_feeTracker))
            {
                _feeTrackers.Remove(_feeTracker);
                _initialElectricCharge.Remove(_feeTracker);
                _feeTracker = null;
            }
            if (_carsWithMeters.ContainsKey(unit))
                _carsWithMeters.Remove(unit);
            if (_newTrackers.ContainsKey(unit))
                _newTrackers.Remove(unit);
            CCLPlugin.LogVerbose($"Removed fee tracker for car {unit.ID}");
        }

        public override void Tick(float delta)
        {
            if (_feeTracker == null)
                return;
            float load = _currentDraw.Value, voltage = _supplyVoltage.Value;
            if (load != 0.0f && !float.IsNaN(load) && !float.IsInfinity(load) && !float.IsNaN(voltage) && !float.IsInfinity(voltage))
            {
                _energyConsumed += load * voltage * _energyConsumptionFactor * delta;
                _electricChargeConsumed.Value = (float) _energyConsumed;
            }
        }

        public override JObject? GetSaveStateData()
        {
            if (_feeTracker == null)
                return null;
            JObject savedData = new();
            savedData.SetDouble("energyConsumed", _energyConsumed);
            //savedData.SetFloat
            return savedData;
        }

        public override void SetSaveStateData(JObject? savedData)
        {
            if (savedData != null)
            {
                _energyConsumed = savedData.GetDouble("energyConsumed") ?? 0.0;
                if (double.IsNaN(_energyConsumed) || double.IsInfinity(_energyConsumed))
                    _energyConsumed = 0.0;
                _electricChargeConsumed.Value = (float) _energyConsumed;
                _feeTracker?.UpdateDebtValues();
            }
        }

        [HarmonyPatch("InitializeDebtComponents")]
        [HarmonyPrefix]
        private static void InitializeDebtComponentsPrefix(DamageController? ___dmgController, 
            Dictionary<ResourceType, List<ResourceContainer>>? ___resourceToResourceContainers, out bool __state)
        {
            __state = false;
            if (___dmgController == null || ___resourceToResourceContainers == null)
                return;
            var unit = TrainCar.Resolve(___dmgController.gameObject);
            if (unit == null || unit.gameObject.GetComponentInChildren<ElectricityMeterDefinitionInternal>() == null)
                return;
            
            __state = true;
            Debug.Log($"EMTR I1 {unit.ID}");
            bool hasElectricChargeContainer = false;
            foreach (KeyValuePair<ResourceType, List<ResourceContainer>> trackedResource in ___resourceToResourceContainers)
            {
                Debug.Log($"EMTR I1 {trackedResource.Key} {trackedResource.Value.Count}");
                if (trackedResource.Key == ResourceType.ElectricCharge)
                {
                    hasElectricChargeContainer = true;
                    break;
                }
            }
            Debug.Log($"EMTR I1 {hasElectricChargeContainer}");
            if (!hasElectricChargeContainer)
                ___resourceToResourceContainers[ResourceType.ElectricCharge] = new();
        }

        [HarmonyPatch("InitializeDebtComponents")]
        [HarmonyPostfix]
        private static void InitializeDebtComponentsPostfix(SimulatedCarDebtTracker? __instance,
            Dictionary<ResourceType, List<ResourceContainer>>? ___resourceToResourceContainers, bool __state)
        {
            if (__state && __instance != null && ___resourceToResourceContainers != null)
            {
                _initialElectricCharge[__instance] = 0.0f;
                foreach (ResourceContainer electricChargeContainer in ___resourceToResourceContainers[ResourceType.ElectricCharge])
                    _initialElectricCharge[__instance] += electricChargeContainer.amountReadOut.Value;
                Debug.Log($"EMTR I2 {_initialElectricCharge[__instance]}");
            }
        }

        [HarmonyPatch("UpdateDebtValues")]
        [HarmonyPrefix]
        private static void UpdateDebtValuesPrefix(SimulatedCarDebtTracker? __instance, out float? __state)
        {
            __state = null;
            if (__instance == null)
                return;

            // Reset the start and snapshot values of the debt component to vanilla settings for consistency
            foreach (DebtComponent currentFee in __instance.GetTrackedDebts())
            {
                if (currentFee.Type == ResourceType.ElectricCharge && _feeTrackers.ContainsKey(__instance))
                { 
                    Debug.Log($"EMTR U1 {currentFee.StartValue} {_initialElectricCharge[__instance]} {currentFee.HasSnapshot} {currentFee.SnapshotValue}");
                    if (currentFee.HasSnapshot)
                        __state = currentFee.StartValue - currentFee.SnapshotValue;
                    currentFee.UpdateStartValue(_initialElectricCharge[__instance]);
                    if (__state != null)
                        currentFee.SetSnapshot(currentFee.StartValue - (float) __state);
                    break;
                }
            }
        }

        [HarmonyPatch("UpdateDebtValues")]
        [HarmonyPostfix]
        private static void UpdateDebtValuesPostfix(SimulatedCarDebtTracker? __instance, float? __state)
        {
            if (__instance == null)
                return;
            foreach (DebtComponent currentFee in __instance.GetTrackedDebts())
            {
                if (currentFee.Type == ResourceType.ElectricCharge && _feeTrackers.TryGetValue(__instance, out ElectricityMeter meter))
                { 
                    float newEndValue = currentFee.EndValue - Mathf.Max((float) meter._energyConsumed, 0.0f);
                    float minimum = (__state != null) ? Mathf.Min(newEndValue, currentFee.SnapshotValue) : newEndValue;
                    if (minimum >= 0.0f)
                        currentFee.UpdateEndValue(newEndValue);
                    else
                    {
                        currentFee.UpdateEndValue(0.0f);
                        currentFee.UpdateStartValue(_initialElectricCharge[__instance] - minimum);
                        if (__state != null)
                            currentFee.SetSnapshot(currentFee.StartValue - (float) __state);
                    }
                    Debug.Log($"EMTR U2 {currentFee.StartValue} {currentFee.EndValue} {currentFee.StartToEndDiff} {__state?.ToString() ?? "<null>"}");
                    break;
                }
            }
        }

        [HarmonyPatch("ResetState"), HarmonyPostfix]
        private static void ResetStatePostfix(SimulatedCarDebtTracker? __instance)
        {
            if (__instance != null && _feeTrackers.TryGetValue(__instance, out ElectricityMeter meter))
            {
                meter._energyConsumed               = 0.0;
                meter._electricChargeConsumed.Value = 0.0f;
                //meter._masterFuse?.ChangeState(false);
            }
        }
    }
}
