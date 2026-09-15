using System.Collections.Generic;

using Newtonsoft.Json.Linq;
using UnityEngine;

using DV.JObjectExtstensions;
using DV.ServicePenalty;
using DV.ThingTypes;
using DV.Utils;
using LocoSim.Implementations;

using CCL.Importer.Components.Simulation.Electric;

namespace CCL.Importer.Implementations
{
    public class ElectricityMeter : SimComponent
    {
        private class PrivateVehicleMeter : LocoDebtTrackerBase
        {
            const float startValue = 524287.0f;
        
            public PrivateVehicleMeter(TrainCar vehicle)
            {
                debtData = new(vehicle.ID, vehicle.carType, InitializeDebtComponents());
            }
        
            public override DebtComponent[] InitializeDebtComponents()
            {
                return new DebtComponent[] 
                { 
                    new(startValue, ResourceType.ElectricCharge) 
                };
            }

            public override bool IsDebtOnlyEnvironmental() => false;

            public override void ResetState()
            {
                if (_feeTrackers.TryGetValue(this, out ElectricityMeter meter))
                {
                    meter.Reset();
                }
            }

            public override void TurnOffDebtSources()
            {
                if (_feeTrackers.TryGetValue(this, out ElectricityMeter meter))
                { 
                    meter._unit?.SimController?.controlsOverrider?.SetNeutralState(); 
                }
            }

            public override void UpdateDebtValues()
            {
                if (_feeTrackers.TryGetValue(this, out ElectricityMeter meter))
                {
                    foreach (DebtComponent current_fee in GetTrackedDebts())
                    {
                        if (current_fee.Type == ResourceType.ElectricCharge)
                        { 
                            current_fee.UpdateEndValue(Mathf.Clamp(startValue - (float) meter._energyConsumed, 0.0f, startValue));
                            break;
                        }
                    }
                }
            }
        }

        private static readonly Dictionary<TrainCar, ElectricityMeter> _carsWithMeters = new();
        private static readonly Dictionary<TrainCar, LocoDebtTrackerBase> _newTrackers = new();
        private static readonly Dictionary<LocoDebtTrackerBase, ElectricityMeter> _feeTrackers = new();
        private static readonly Dictionary<SimulatedCarDebtTracker, float> _initialElectricCharge = new();
        
        private readonly TrainCar? _unit;
        private LocoDebtTrackerBase? _feeTracker;
        
        private readonly Port _electricChargeConsumed;

        private readonly PortReference _supplyVoltage;
        private readonly PortReference _currentDraw;

        private float _energyConsumptionFactor;
        private double _energyConsumed = 0.0;

        public static Dictionary<SimulatedCarDebtTracker, float> initialElectricCharge => _initialElectricCharge;
        public static Dictionary<LocoDebtTrackerBase, ElectricityMeter> feeTrackers => _feeTrackers;
        
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

        internal static void AssignNewTracker(TrainCar vehicle, SimulatedCarDebtTracker? standardTracker)
        {
            if (!vehicle.playerSpawnedCar)
            {
                bool trackerAssigned;
                if (vehicle.uniqueCar)
                {
                    _newTrackers[vehicle] = new PrivateVehicleMeter(vehicle);
                    SingletonBehaviour<LocoDebtController>.Instance.RegisterLocoDebtTracker(vehicle, _newTrackers[vehicle]);
                    trackerAssigned = true;
                }
                else if (standardTracker != null)
                { 
                    _newTrackers[vehicle] = standardTracker; 
                    trackerAssigned = true;
                }
                else
                { 
                    trackerAssigned = false;
                }
                if (trackerAssigned && _carsWithMeters.TryGetValue(vehicle, out ElectricityMeter meter))
                {
                    meter.TrySetupFeeTracker();
                }
            }
        }

        internal static void ReplaceTrackerOnOwnershipChange(TrainCar vehicle, SimulatedCarDebtTracker? standardTracker)
        { 
            TrainCar? unit = null;
            ElectricityMeter? meter = null;
            foreach (KeyValuePair<LocoDebtTrackerBase, ElectricityMeter> currentTracker in _feeTrackers)
            {
                meter = currentTracker.Value;
                unit = meter._unit;
                if (unit == vehicle && (currentTracker.Key is PrivateVehicleMeter) != unit.uniqueCar)
                {
                    meter.DisposeFeeTracker(unit);
                    if (!unit.uniqueCar)
                    {   
                        meter.Reset();      // Any leftover electricity bill on a privately owned vehicle is staged and documented by the call above
                    }
                    if (unit.uniqueCar || standardTracker != null)
                    {
                        CCLPlugin.LogVerbose($"Re-registering fee tracker for {(unit.uniqueCar ? "private" : "DVRT")} vehicle {unit.ID}");
                        _carsWithMeters[unit] = meter;
                        AssignNewTracker(unit, standardTracker);
                    }
                    break;
                }
            }
        }

        private void TrySetupFeeTracker()
        {
            if (_unit != null && _carsWithMeters.ContainsKey(_unit) && _newTrackers.TryGetValue(_unit, out _feeTracker))
            {
                _feeTrackers[_feeTracker] = this;
                _newTrackers.Remove(_unit);
                _feeTracker.UpdateDebtValues();
                CCLPlugin.LogVerbose($"Set up a fee tracker <{_feeTracker.GetType()}> for car {_unit.ID}");
            }
        }

        private void DisposeFeeTracker(TrainCar unit)
        {
            unit.OnDestroyCar -= DisposeFeeTracker;
            if (_feeTracker != null && _feeTrackers.ContainsKey(_feeTracker))
            {
                _feeTrackers.Remove(_feeTracker);
                if (_feeTracker is PrivateVehicleMeter)
                { 
                    SingletonBehaviour<LocoDebtController>.Instance.StageLocoDebtOnLocoDestroy(_feeTracker);
                    CCLPlugin.LogVerbose($"Staged remaining fees on car {unit.ID}");
                }
                else if (_feeTracker is SimulatedCarDebtTracker standardTracker)
                {
                    _initialElectricCharge.Remove(standardTracker);
                }
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
