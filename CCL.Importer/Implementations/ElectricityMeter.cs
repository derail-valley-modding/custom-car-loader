using System.Collections.Generic;

using HarmonyLib;
using Newtonsoft.Json.Linq;
using UnityEngine;

using DV.Damage;
using DV.JObjectExtstensions;
using DV.ServicePenalty;
using DV.Simulation.Cars;
using DV.ThingTypes;
using LocoSim.Implementations;

using CCL.Importer.Components.Simulation.Electric;

namespace CCL.Importer.Implementations
{
    public class ElectricityMeter : SimComponent
    {
        private static readonly Dictionary<TrainCar, ElectricityMeter> _carsWithMeters = new();
        private static readonly Dictionary<TrainCar, SimulatedCarDebtTracker> _newTrackers = new();
        private static readonly Dictionary<SimulatedCarDebtTracker, ElectricityMeter> _feeTrackers = new();
        private static readonly Dictionary<SimulatedCarDebtTracker, float> _initialElectricCharge = new();
        
        private readonly TrainCar? _unit;
        private SimulatedCarDebtTracker? _feeTracker;
        
        private readonly Port _electricChargeConsumed;

        private readonly PortReference _supplyVoltage;
        private readonly PortReference _currentDraw;

        private float _energyConsumptionFactor;
        private double _energyConsumed = 0.0;

        public static Dictionary<SimulatedCarDebtTracker, float> initialElectricCharge => _initialElectricCharge;
        public static Dictionary<SimulatedCarDebtTracker, ElectricityMeter> feeTrackers => _feeTrackers;
        
        public double energyConsumed => _energyConsumed;
        public override bool HasSaveData => true;
        
        public ElectricityMeter(ElectricityMeterDefinitionInternal definition) : base(definition.ID)
        {
            _energyConsumptionFactor = definition.electricChargeConsumptionFactor / (1000.0f * 3600.0f);

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
            {
                _unit.LogicCarInitialized += AdjustEnergyConsumptionFactor;
            }
            else
            {
                AdjustEnergyConsumptionFactor();
            }
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

        internal static void AddNewTracker(TrainCar vehicle, SimulatedCarDebtTracker newTracker)
        {
            _newTrackers[vehicle] = newTracker;
            if (_carsWithMeters.TryGetValue(vehicle, out ElectricityMeter meter))
            {
                meter.TrySetupFeeTracker();
            }
        }

        private void TrySetupFeeTracker()
        {
            if (_unit == null || !_carsWithMeters.ContainsKey(_unit) || !_newTrackers.ContainsKey(_unit))
            {
                return;
            }
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
            {
                _carsWithMeters.Remove(unit);
            }
            if (_newTrackers.ContainsKey(unit))
            { 
                _newTrackers.Remove(unit); 
            }
            CCLPlugin.LogVerbose($"Removed fee tracker for car {unit.ID}");
        }

        public override void Tick(float delta)
        {
            if (_feeTracker == null)
            { 
                return; 
            }
            float load = _currentDraw.Value, voltage = _supplyVoltage.Value;
            if (load != 0.0f && !float.IsNaN(load) && !float.IsInfinity(load) && !float.IsNaN(voltage) && !float.IsInfinity(voltage))
            {
                _energyConsumed += load * voltage * _energyConsumptionFactor * delta;
                _electricChargeConsumed.Value = (float) _energyConsumed;
            }
        }

        internal void Reset()
        {
            _energyConsumed = 0.0;
            _electricChargeConsumed.Value = 0.0f;
        }

        public override JObject? GetSaveStateData()
        {
            if (_feeTracker == null)
            { 
                return null; 
            }
            JObject savedData = new();
            savedData.SetDouble("energyConsumed", _energyConsumed);
            return savedData;
        }

        public override void SetSaveStateData(JObject? savedData)
        {
            if (savedData != null)
            {
                _energyConsumed = savedData.GetDouble("energyConsumed") ?? 0.0;
                if (double.IsNaN(_energyConsumed) || double.IsInfinity(_energyConsumed))
                { 
                    _energyConsumed = 0.0; 
                }
                _electricChargeConsumed.Value = (float) _energyConsumed;
                _feeTracker?.UpdateDebtValues();
            }
        }
    }
}
