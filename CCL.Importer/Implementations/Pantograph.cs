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
        
        private readonly FuseReference _powerFuse;
        private readonly Port _wireHeight, _initialHeadHeight, _headHeight, _raiseReadOut, _raiseNormalizedReadOut;
        private readonly Port _wireVoltage, _voltageReadOut;
        private readonly Port _inContact;
        private readonly PortReference _pantographToggle;
        private readonly TrainCar?  _unit;

        private readonly float _maximumRaise, _headMovementSpeed, _contactTolerance;
        
        private bool _disabled = false;
        private float _minimumRaise = 0.0f, _maximumRaiseDifference;

        public Pantograph(PantographDefinitionInternal definition): base(definition.ID)
        {
            _headMovementSpeed = definition.headMovementSpeed;
            _maximumRaise = definition.maximumRaise;
            _contactTolerance  = definition.contactTolerance;

            _powerFuse = AddFuseReference(definition.powerFuseId);

            _wireHeight = AddPort(definition.wireHeight);
            _initialHeadHeight = AddPort(definition.initialHeadHeight);
            _headHeight = AddPort(definition.headHeight);
            _wireVoltage = AddPort(definition.wireVoltage);
            _voltageReadOut = AddPort(definition.supplyVoltage);
            _raiseReadOut = AddPort(definition.pantographRaise);
            _raiseNormalizedReadOut = AddPort(definition.pantographRaiseNormalized);
            _inContact = AddPort(definition.pantographInContact);
            _initialHeadHeight.ValueUpdatedInternally += CheckInitialHeight;

            _pantographToggle = AddPortReference(definition.toggle);

            CheckInitialHeight(_initialHeadHeight.Value);
            if (_disabled)
            { 
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
                installedPantographs.Add(this);
            }
            else
            {
                unit.OnDestroyCar += OnCarDestroyed;
            }
        }

        private void CheckInitialHeight(float initialHeight)
        {
            if (_disabled)
            { 
                return; 
            }
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
            { 
                return; 
            }
            unit.OnDestroyCar -= OnCarDestroyed;
            foreach (Pantograph currentPantograph in _allPantographs[unit])
            {
                currentPantograph._disabled = true;
                currentPantograph._initialHeadHeight.ValueUpdatedInternally -= currentPantograph.CheckInitialHeight;
            }
            _allPantographs[unit].Clear();
            _allPantographs.Remove(unit);
        }

        private bool IsInContact(float wireHeight, bool pantographOn)
        {
            return wireHeight >= 0.0f && Mathf.Abs(wireHeight - _headHeight.Value) <= _contactTolerance;
        }
        
        private void Move(float delta, float raiseHeight, bool pantographOn)
        {
            if (_disabled)
            { 
                return; 
            }
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
            { 
                return; 
            }
            bool pantographOn = _pantographToggle.Value >= 0.5f && _powerFuse.State;
            float wireHeight = _wireHeight.Value;
            float raiseHeight;
            if (!pantographOn)
            {
                raiseHeight = _minimumRaise;
            }
            else
            { 
                raiseHeight = (wireHeight > 0.0f) ? wireHeight : _maximumRaise; 
            }
            Move(delta, raiseHeight, pantographOn);
            if (!IsInContact(wireHeight, pantographOn))
            {
                _inContact.Value = _voltageReadOut.Value = 0.0f;
            }
            else
            {
                _inContact.Value = 1.0f;
                _voltageReadOut.Value = _wireVoltage.Value;
            }
		}
    }
}
