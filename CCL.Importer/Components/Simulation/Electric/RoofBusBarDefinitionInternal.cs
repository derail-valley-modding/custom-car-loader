using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CCL.Importer.Implementations;

using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation.Electric
{
    public class RoofBusBarDefinitionInternal : SimComponentDefinition
    {
        public float nominalVoltage = 1500.0f;
        public int pantographCount = 1;

        public PortReferenceDefinition[]? inputsFromPantographs;

        public readonly PortDefinition supplyVoltage = new(PortType.READONLY_OUT, PortValueType.VOLTS, "SUPPLY_VOLTAGE");
        public readonly PortDefinition supplyVoltageNormalized = new(PortType.READONLY_OUT, PortValueType.VOLTS, "SUPPLY_VOLTAGE_NORMALIZED");
        public readonly PortDefinition pantographsInputCurrent = new(PortType.READONLY_OUT, PortValueType.AMPS, "PANTOGRAPHS_INPUT_CURRENT");
        
        public override SimComponent InstantiateImplementation()
        {
            Debug.Log($"RBB {inputsFromPantographs?.Length.ToString() ?? "<null>"}");
            if (inputsFromPantographs != null)
            {
                for (int index = 0; index < inputsFromPantographs.Length; ++index)
                    Debug.Log($"RBB [{index}] {inputsFromPantographs[index].ID} {inputsFromPantographs[index].valueType}");
            }
            return new RoofBusBar(this);
        }
    }
}
