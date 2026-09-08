using System;
using System.Linq;

using UnityEngine;

using LocoSim.Definitions;
using LocoSim.Implementations;

using CCL.Importer.Components.Simulation.Electric;

namespace CCL.Importer.Implementations
{
    internal class RoofBusBar : SimComponent
    {
        private readonly float _nominalVoltage;
        private readonly Port _supplyVoltage, _supplyVoltageNormalized, _pantographInputCurrent, _raisedCount;
        private readonly Port[]? _inContact;
        private readonly PortReference[]? _inputsFromPantographs, _pantographVoltages;
        private readonly PortReference? _currentDraw;
        private readonly bool[]? _pantographRaised;

        private int _raisedPantographsCount = 0;

        private Action<float> CreateContactHandler(int pantographIndex)
        {
            return delegate (float contactState)
            {
                bool nowInContact = contactState >= 0.5f;
                if (_pantographRaised![pantographIndex] != nowInContact)
                {
                    if (nowInContact)
                    {
                        _raisedCount.Value = ++_raisedPantographsCount;
                    }
                    else
                    { 
                        _raisedCount.Value = --_raisedPantographsCount; 
                    }
                }
                _pantographRaised[pantographIndex] = nowInContact;
            };
        }

        public RoofBusBar(RoofBusBarDefinitionInternal definition) : base(definition.ID)
        {
            _nominalVoltage = definition.nominalVoltage;
            
            _supplyVoltage = AddPort(definition.supplyVoltage);
            _supplyVoltageNormalized = AddPort(definition.supplyVoltageNormalized);
            _pantographInputCurrent = AddPort(definition.pantographInputCurrent);
            _raisedCount = AddPort(definition.raisedPantographsCount);
            
            if (_nominalVoltage <= 0.0f)
            { 
                CCLPlugin.Error("Bus bar nominal voltage negative or zero, overhead power will not be collected"); 
                return;
            }
            if (definition.allInputs == null || definition.allInputs.Length < 3 || (definition.allInputs.Length & 1) == 0)
            { 
                CCLPlugin.Error("Bus bar has no or invalid number of inputs, overhead power will not be collected"); 
                return;
            }
            
            PortReferenceDefinition[] allInputs = definition.allInputs;
            int lastInputIndex = definition.allInputs.Length - 1;
            _inputsFromPantographs =
            (
                from index in Enumerable.Range(0, lastInputIndex)
                select AddPortReference(allInputs[index])
            ).ToArray();
            _currentDraw = AddPortReference(allInputs[lastInputIndex]);
            int pantographsCount = _inputsFromPantographs.Length / 2;
            _inContact = new Port[pantographsCount];
            _pantographRaised = new bool[pantographsCount];
            _pantographVoltages = 
            (
                from pantographIndex in Enumerable.Range(0, pantographsCount)
                select _inputsFromPantographs[pantographIndex * 2 + 1]
            ).ToArray();
        }

        public override void InitializationAfterConnecting()
        {
            if (_inputsFromPantographs != null && _inContact != null && _pantographVoltages != null)
            {
                for (int pantographIndex = 0; pantographIndex < _inContact.Length; pantographIndex++)
                {
                    _inContact[pantographIndex] = _inputsFromPantographs[pantographIndex * 2].GetPort();
                    Action<float> ContactHandler = CreateContactHandler(pantographIndex);
                    _inContact[pantographIndex].ValueUpdatedInternally += ContactHandler;
                    ContactHandler(_inContact[pantographIndex].Value);
                }
            }
        }

        public override void Tick(float delta)
        {
            if (_raisedPantographsCount == 0)
            {
                _supplyVoltage.Value = _supplyVoltageNormalized.Value = _pantographInputCurrent.Value = 0.0f;
            }
            else
            {
                float voltage = 0.0f;
                for (int pantographIndex = 0; pantographIndex < _pantographVoltages!.Length; pantographIndex++)
                {
                    if (_pantographRaised![pantographIndex])
                    { 
                        voltage = Mathf.Max(voltage, _pantographVoltages[pantographIndex].Value); 
                    }
                }
                _supplyVoltage.Value = voltage;
                _supplyVoltageNormalized.Value = voltage / _nominalVoltage;
                _pantographInputCurrent.Value = _currentDraw!.Value / _raisedPantographsCount;
            }
        }
    }
}
