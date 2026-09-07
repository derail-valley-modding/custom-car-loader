using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CCL.Importer.Components.Simulation.Electric;
using CCL.Types.Components.Simulation.Electric;

using LocoSim.Implementations;

namespace CCL.Importer.Implementations
{
    internal class RoofBusBar : SimComponent
    {
        private readonly float _nominalVoltage;
        private readonly Port _supplyVoltage, _supplyVoltageNormalized, _pantographInputCurrent;
        private readonly Port[]? _inContact;
        private readonly PortReference[]? _inputsFromPantographs, _pantographVoltages;
        private readonly bool[]? _raisedPantographs;

        private int _raisedPantographsCount = 0;

        private Action<float> CreateContactHandler(int pantograph)
        {
            return delegate (float contactState)
            {
                bool nowInContact = contactState >= 0.5f;
                if (_raisedPantographs![pantograph] != nowInContact)
                {
                    if (nowInContact)
                        _raisedPantographsCount++;
                    else
                        _raisedPantographsCount--;
                }
                _raisedPantographs[pantograph] = nowInContact;
            };
        }

        public RoofBusBar(RoofBusBarDefinitionInternal definition) : base(definition.ID)
        {
            _supplyVoltage = AddPort(definition.supplyVoltage);
            _supplyVoltageNormalized = AddPort(definition.supplyVoltageNormalized);
            _pantographInputCurrent = AddPort(definition.pantographsInputCurrent);
            
            if (definition.nominalVoltage <= 0.0f)
            { 
                CCLPlugin.Error("Nominal voltage negative or zero, overhead power will not be collected"); 
                return;
            }
            if (definition.inputsFromPantographs == null || definition.inputsFromPantographs.Length == 0)
            { 
                CCLPlugin.Error("Bus bar has no inputs set"); 
                return;
            }
            
            _inputsFromPantographs =
            (
                from currentReference in definition.inputsFromPantographs
                select AddPortReference(currentReference)
            ).ToArray();
            Debug.Log($"RBB {_inputsFromPantographs.Length}");
            for (int index = 0; index < _inputsFromPantographs.Length; ++index)
                Debug.Log($"RBB [{index}] {_inputsFromPantographs[index].id}");
            int pantographsCount = _inputsFromPantographs.Length / 2;
            _inContact = new Port[pantographsCount];
            _raisedPantographs = new bool[pantographsCount];
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
                int pantographsCount = _inputsFromPantographs.Length / 2;
                for (int pantographIndex = 0; pantographIndex < pantographsCount; pantographIndex++)
                {
                    _inContact[pantographIndex] = _inputsFromPantographs[pantographIndex * 2].GetPort();
                    Debug.Log($"RBB [{pantographIndex}] {_inContact[pantographIndex]?.id ?? "<null>"} {_inContact[pantographIndex]?.type.ToString() ?? "<null>"} {_inContact[pantographIndex]?.valueType.ToString() ?? "<null>"} {_pantographVoltages[pantographIndex]?.id ?? "<null>"}");
                }
            }
		}

        public override void Tick(float delta)
        {}
    }
}
