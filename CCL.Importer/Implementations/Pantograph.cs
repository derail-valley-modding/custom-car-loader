using System.Collections.Generic;

using UnityEngine;

using LocoSim.Implementations;

using CCL.Importer.Components.Simulation.Electric;

namespace CCL.Importer.Implementations
{
    internal class Pantograph : PowerCollectorCommonPorts
    {
        private static readonly Dictionary<TrainCar, List<Pantograph>> _allPantographs = new();
        
        private readonly FuseReference _powerFuse;
        private readonly Port _raiseReadOut;
        private readonly Port _raiseNormalizedReadOut;
        private readonly PortReference _pantographToggle;
        private readonly TrainCar?  _unit;

        private readonly float _maximumRaise;
        private readonly float _headMovementSpeed;
        
        private bool _disabled = false;
        private float _minimumRaise = 0.0f;
        private float _maximumRaiseDifference;

        public Pantograph(PantographDefinitionInternal definition): base(definition)
        {
            _headMovementSpeed = definition.headMovementSpeed;
            _maximumRaise = definition.maximumRaise;

            _powerFuse = AddFuseReference(definition.powerFuseId);
            _raiseReadOut = AddPort(definition.pantographRaise);
            _raiseNormalizedReadOut = AddPort(definition.pantographRaiseNormalized);
            _pantographToggle = AddPortReference(definition.toggle);
            _initialHeadHeight.ValueUpdatedInternally += CheckInitialHeight;

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
            if (!_disabled)
            { 
                if (_maximumRaise <= initialHeight)
                {
                    CCLPlugin.Error("Maximum reach is below initial position, pantograph disabled");
                    _disabled = true;
                    return;
                }
                _minimumRaise = initialHeight;
                _maximumRaiseDifference = _maximumRaise - initialHeight;
            }
        }
        
        private void OnCarDestroyed(TrainCar? unit)
        {
            if (unit != null && _allPantographs.TryGetValue(unit, out List<Pantograph> installedPantographs))
            { 
                unit.OnDestroyCar -= OnCarDestroyed;
                foreach (Pantograph currentPantograph in installedPantographs)
                {
                    currentPantograph._disabled = true;
                    currentPantograph._initialHeadHeight.ValueUpdatedInternally -= currentPantograph.CheckInitialHeight;
                }
                installedPantographs.Clear();
                _allPantographs.Remove(unit);
            }
        }

        private void Move(float delta, float raiseHeight, bool pantographOn)
        {
            const float proximitySlowdown = 0.2f;

            float currentRaise = _raiseReadOut.Value + _minimumRaise;
            float targetRaise;
            float raiseDifference;
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
                float movementSpeed = Mathf.Min(_headMovementSpeed, raiseDifference / proximitySlowdown);
                currentRaise = Mathf.Min(currentRaise + movementSpeed * delta, _maximumRaise);
                _raiseReadOut.Value = currentRaise - _minimumRaise;
                _raiseNormalizedReadOut.Value = Mathf.Clamp((currentRaise - _minimumRaise) / _maximumRaiseDifference, 0.0f, 0.999f);
            }
            else if (raiseDifference < -0.006f)
            {
                float movementSpeed = Mathf.Min(_headMovementSpeed, raiseDifference / (-proximitySlowdown));
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
            _voltageReadOut.Value = (_inContact.Value >= 0.5f) ? _wireVoltage.Value : 0.0f;
        }
    }
}
