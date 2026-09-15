using LocoSim.Implementations;

using CCL.Importer.Components.Simulation.Electric;

namespace CCL.Importer.Implementations
{
    internal abstract class PowerCollectorCommonPorts : SimComponent
    {
        protected readonly Port _wireHeight;
        protected readonly Port _initialHeadHeight;
        protected readonly Port _headHeight;
        protected readonly Port _wireVoltage;
        protected readonly Port _voltageReadOut;
        protected readonly Port _inContact;

        protected PowerCollectorCommonPorts(PowerCollectorCommonPortsDefinitionInternal definition): base(definition.ID)
        {
            _wireHeight = AddPort(definition.wireHeight);
            _initialHeadHeight = AddPort(definition.initialHeadHeight);
            _headHeight = AddPort(definition.headHeight);
            _wireVoltage = AddPort(definition.wireVoltage);
            _voltageReadOut = AddPort(definition.supplyVoltage);
            _inContact = AddPort(definition.pantographInContact);
        }
    }
}
