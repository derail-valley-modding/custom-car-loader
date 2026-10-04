using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation
{
    internal class DoubledControlDefinitionInternal : SimComponentDefinition
    {
        public float DefaultValue;

        public readonly PortDefinition ControlAIn = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "CONTROL_A_EXT_IN");
        public readonly PortDefinition ControlBIn = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "CONTROL_B_EXT_IN");
        public readonly PortDefinition CombinedIn = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "COMBINED_EXT_IN");
        public readonly PortDefinition EffectA = new(PortType.READONLY_OUT, PortValueType.GENERIC, "EFFECT_A_OUT");
        public readonly PortDefinition EffectB = new(PortType.READONLY_OUT, PortValueType.GENERIC, "EFFECT_B_OUT");

        public readonly PortReferenceDefinition EffectReader = new(PortValueType.GENERIC, "EFFECT");

        public override SimComponent InstantiateImplementation()
        {
            return new DoubledControl(this);
        }
    }
}
