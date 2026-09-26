using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation
{
    internal class SteppableControlHandlerDefinitionInternal : SimComponentDefinition
    {
        public float Step = 0.1f;
        public float FastStepTime = 0.2f;

        public readonly PortDefinition ControlIn = new(PortType.EXTERNAL_IN, PortValueType.CONTROL, "EXT_IN");
        public readonly PortReferenceDefinition OtherControlIn = new(PortValueType.CONTROL, "OTHER_CONTROL_EXT_IN", true);

        public override SimComponent InstantiateImplementation()
        {
            return new SteppableControlHandler(this);
        }
    }
}
