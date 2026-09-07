using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CCL.Types.Json;
using CCL.Types.Proxies.Ports;

using UnityEngine;

namespace CCL.Types.Components.Simulation.Electric
{
    [AddComponentMenu("CCL/Components/Simulation/Electric/Roof Bus Bar Definition")]
    public class RoofBusBarDefinition : SimComponentDefinitionProxy, ICustomSerialized
    {
        [Min(1.0f), Tooltip("Used to calculate normalized voltage port")]
        public float nominalVoltage = 1500.0f;
        
        [Delayed]
        public int pantographCount = 1;
        
        private PortReferenceDefinition[]? _inputsFromPantographs;
        [SerializeField, HideInInspector]
        private string? _savedInputs;

        public PortReferenceDefinition[]? inputsFromPantographs => _inputsFromPantographs;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, "SUPPLY_VOLTAGE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, "SUPPLY_VOLTAGE_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.AMPS, "PANTOGRAPHS_INPUT_CURRENT")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => _inputsFromPantographs ?? base.ExposedPortReferences;

        public override void OnValidate()
        {
            base.OnValidate();

            if (_inputsFromPantographs == null || _inputsFromPantographs.Length != pantographCount)
            {
                _inputsFromPantographs = new PortReferenceDefinition[pantographCount * 2];
                for (int pantograph = 0; pantograph < pantographCount; pantograph++)
                {
                    _inputsFromPantographs[pantograph * 2] = new PortReferenceDefinition(DVPortValueType.STATE, $"PANTOGRAPH_IN_CONTACT_{pantograph}");
                    _inputsFromPantographs[pantograph * 2 + 1] = new PortReferenceDefinition(DVPortValueType.VOLTS, $"PANTOGRAPH_VOLTAGE_{pantograph}");
                }
            }
            _savedInputs = JSONObject.ToJson(_inputsFromPantographs);
        }
        
        public void AfterImport()
        {
            _inputsFromPantographs = string.IsNullOrWhiteSpace(_savedInputs) ? null : JSONObject.FromJson<PortReferenceDefinition[]>(_savedInputs);
        }
    }
}
