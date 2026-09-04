using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

using LocoSim.Implementations;

using CCL.Importer.Components.Simulation.Electric;

namespace CCL.Importer.Implementations
{
    internal class Pantograph : SimComponent
    {
        private static readonly Dictionary<TrainCar, List<Pantograph>> _allPantographs = new();
        private static readonly Dictionary<TrainCar, int> _nextPantographID = new(), _raisedPantographMask = new(), _raisedPantographCount = new();
        
        private readonly FuseReference _powerFuse;
        private readonly Port _wireHeight, _initialHeadHeight, _headHeight, _raiseReadOut, _raiseNormalizedReadOut;
        private readonly Port _wireVoltage, _voltageReadOut, _voltageNormalizedReadOut;
        private readonly PortReference _pantographToggle;
        private readonly TrainCar?  _unit;

        private readonly float _nominalVoltage;
        private readonly float _maximumRaise, _headMovementSpeed, _contactTolerance;
        private readonly int _IDMask, _IDInvertedMask;
        
        private bool _disabled = false, _isInContact = false;
        private float _minimumRaise = 0.0f, _maximumRaiseDifference;

        public static int? RaisedPantogrpahsCount(TrainCar? vehicle)
        {
            return (vehicle == null || !_raisedPantographCount.TryGetValue(vehicle, out int raisedPantographs)) ? null : raisedPantographs;
        }

        public Pantograph(PantographDefinitionInternal definition): base(definition.ID)
        {
            _nominalVoltage = definition.nominalVoltage;
            _headMovementSpeed = definition.headMovementSpeed;
            _maximumRaise = definition.maximumRaise;
            _contactTolerance  = definition.contactTolerance;

            _powerFuse = AddFuseReference(definition.powerFuseId);
            _wireHeight = AddPort(definition.wireHeight);
            _initialHeadHeight = AddPort(definition.initialHeadHeight);
            _headHeight = AddPort(definition.headHeight);
            _wireVoltage = AddPort(definition.wireVoltage);
            _voltageReadOut = AddPort(definition.supplyVoltage);
            _voltageNormalizedReadOut = AddPort(definition.supplyVoltageNormalized);
            _raiseReadOut = AddPort(definition.pantographRaise);
            _raiseNormalizedReadOut = AddPort(definition.pantographRaiseNormalized);
            _pantographToggle = AddPortReference(definition.toggle);
            _initialHeadHeight.ValueUpdatedInternally += CheckInitialHeight;

            CheckInitialHeight(_initialHeadHeight.Value);
            if (_disabled)
                return;
            if (_nominalVoltage <= 0.0f)
            {
                CCLPlugin.Error("Nominal voltage negative or zero, pantograph disabled");
                _disabled = true;
                return;
            }
            if (_headMovementSpeed <= 0.0f)
            {
                CCLPlugin.Error("Head movement speed negative or zero, pantograph disabled");
                _disabled = true;
                return;
            }
            _unit = TrainCar.Resolve(definition.gameObject);
            TrainCar? unit = _unit;
            if (unit == null)
            {
                CCLPlugin.Error("Car not resolved, pantograph disabled");
                _disabled = true;
                return;
            }
            
            if (_allPantographs.TryGetValue(unit, out List<Pantograph> installedPantographs))
            {
                if (_nextPantographID[unit] >= 30)
                {
                    CCLPlugin.Error("Cannot have more than 30 pantographs on a car");
                    _disabled = true;
                    return;
                }
                _IDMask = 1 << _nextPantographID[unit]++;
                installedPantographs.Add(this);
            }
            else
            {
                _IDMask = 1;
                _allPantographs[unit] = new() { this };
                _nextPantographID[unit] = 1;
                _raisedPantographMask[unit] = _raisedPantographCount[unit] = 0;
                unit.OnDestroyCar += OnCarDestroyed;
            }
            _IDInvertedMask = ~_IDMask;
        }

        private void CheckInitialHeight(float initialHeight)
        {
            if (_disabled)
                return;
            if (_maximumRaise <= initialHeight)
            {
                CCLPlugin.Error("Maximum reach is below initial position, pantograph disabled");
                _disabled = true;
                return;
            }
            _minimumRaise = initialHeight;
            _maximumRaiseDifference = _maximumRaise - initialHeight;
        }
        
        private void OnCarDestroyed(TrainCar unit)
        {
            if (unit == null || !_allPantographs.ContainsKey(unit))
                return;
            unit.OnDestroyCar -= OnCarDestroyed;
            foreach (Pantograph currentPantograph in _allPantographs[unit])
            {
                currentPantograph._disabled = true;
                currentPantograph._initialHeadHeight.ValueUpdatedInternally -= currentPantograph.CheckInitialHeight;
            }
            _allPantographs[unit].Clear();
            _allPantographs.Remove(unit);
            _nextPantographID.Remove(unit);
            _raisedPantographMask.Remove(unit);
            _raisedPantographCount.Remove(unit);
        }

        private bool TrackContactState(float wireHeight, bool pantographOn)
        {
            TrainCar unit = _unit!;
            if (_disabled)
                return false;
            bool wasInContact = (_raisedPantographMask[unit] & _IDMask) != 0;
            bool nowInContact;
            if (!pantographOn || wireHeight < 0.0f)
                nowInContact = false;
            else
                nowInContact = Mathf.Abs(wireHeight - _headHeight.Value) <= _contactTolerance;
            if (nowInContact != wasInContact)
            {
                if (nowInContact)
                {
                    _raisedPantographMask [unit] |= _IDMask;
                    _raisedPantographCount[unit]++;
                }
                else
                {
                    _raisedPantographMask [unit] &= _IDInvertedMask;
                    _raisedPantographCount[unit]--;
                }
            }
            return nowInContact;
        }
        
        private void Move(float delta, float raiseHeight, bool pantographOn)
        {
            if (_disabled)
                return;
            float currentRaise = _raiseReadOut.Value + _minimumRaise;
            float targetRaise, raiseDifference;
            if (pantographOn)
            {
                targetRaise = raiseHeight;
                raiseDifference = targetRaise - _headHeight.Value;
            }
            else
            {
                targetRaise = _minimumRaise;
                raiseDifference = targetRaise - currentRaise;
            }
            if (raiseDifference > 0.006f)
            {
                float movementSpeed = Mathf.Min(_headMovementSpeed, raiseDifference / 0.2f);
                currentRaise = Mathf.Min(currentRaise + movementSpeed * delta, _maximumRaise);
                _raiseReadOut.Value = currentRaise - _minimumRaise;
                _raiseNormalizedReadOut.Value = Mathf.Clamp((currentRaise - _minimumRaise) / _maximumRaiseDifference, 0.0f, 0.999f);
            }
            else if (raiseDifference < -0.006f)
            {
                float movementSpeed = Mathf.Min(_headMovementSpeed, raiseDifference / (-0.2f));
                currentRaise = Mathf.Max(currentRaise - movementSpeed * delta, _minimumRaise);
                _raiseReadOut.Value = currentRaise - _minimumRaise;
                _raiseNormalizedReadOut.Value = Mathf.Clamp((currentRaise - _minimumRaise) / _maximumRaiseDifference, 0.0f, 0.999f);
            }
        }

        public override void Tick(float delta)
        {
            if (_disabled)
                return;
            bool pantographOn = _pantographToggle.Value >= 0.5f && _powerFuse.State;
            float wireHeight = _wireHeight.Value;
            float raiseHeight;
            if (!pantographOn)
                raiseHeight = _minimumRaise;
            else
                raiseHeight = (wireHeight > 0.0f) ? wireHeight : _maximumRaise;
            Move(delta, raiseHeight, pantographOn);
            _isInContact = TrackContactState(wireHeight, pantographOn);
            if (!_isInContact)
                _voltageReadOut.Value = _voltageNormalizedReadOut.Value = 0.0f;
            else
            {
                float voltage = _wireVoltage.Value;
                _voltageReadOut.Value = voltage;
                _voltageNormalizedReadOut.Value = voltage / _nominalVoltage;
            }
        }
    }
}
