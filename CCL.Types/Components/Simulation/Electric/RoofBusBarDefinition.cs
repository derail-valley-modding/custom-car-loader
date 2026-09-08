using System.Collections.Generic;

using UnityEngine;

using CCL.Types.Json;
using CCL.Types.Proxies.Ports;

namespace CCL.Types.Components.Simulation.Electric
{
    [AddComponentMenu("CCL/Components/Simulation/Electric/Roof Bus Bar Definition")]
    public class RoofBusBarDefinition : SimComponentDefinitionProxy, ICustomSerialized
    {
        [Min(1.0f), Tooltip("Used to calculate normalized voltage port")]
        public float nominalVoltage = 1500.0f;
        
        [Delayed]
        public int pantographCount = 1;
        
        private PortReferenceDefinition[]? _allInputs;
        
        [SerializeField, HideInInspector]
        private string? _savedInputs;

        public PortReferenceDefinition[]? allInputs => _allInputs;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, "SUPPLY_VOLTAGE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, "SUPPLY_VOLTAGE_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.AMPS, "PANTOGRAPH_INPUT_CURRENT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "PANTOGRAPHS_RAISED_COUNT")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => _allInputs ?? base.ExposedPortReferences;

        public override void OnValidate()
        {
            base.OnValidate();

            if (_allInputs == null || _allInputs.Length != pantographCount)
            {
                _allInputs = new PortReferenceDefinition[pantographCount * 2 + 1];
                for (int pantograph = 0; pantograph < pantographCount; pantograph++)
                {
                    _allInputs[pantograph * 2] = new PortReferenceDefinition(DVPortValueType.STATE, $"PANTOGRAPH_IN_CONTACT_{pantograph}");
                    _allInputs[pantograph * 2 + 1] = new PortReferenceDefinition(DVPortValueType.VOLTS, $"PANTOGRAPH_VOLTAGE_{pantograph}");
                }
                _allInputs[pantographCount * 2] = new PortReferenceDefinition(DVPortValueType.AMPS, "CURRENT_DRAW");
            }
            _savedInputs = JSONObject.ToJson(_allInputs);
        }
        
        public void AfterImport()
        {
            _allInputs = string.IsNullOrWhiteSpace(_savedInputs) ? null : JSONObject.FromJson<PortReferenceDefinition[]>(_savedInputs);
        }
    }
}
