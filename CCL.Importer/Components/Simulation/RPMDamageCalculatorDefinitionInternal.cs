using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Components.Simulation
{
    internal class RPMDamageCalculatorDefinitionInternal : SimComponentDefinition
    {
        public float MaxRPM;
        public float DamagePerSecond = 0.005f;
        public float OverspeedDamagePerSecond = 0.005f;
        public float ScalingFactor = 50.0f;

        public readonly PortReferenceDefinition RPMReader = new(PortValueType.RPM, "RPM", false);
        public readonly PortDefinition RPMNormalised = new(PortType.READONLY_OUT, PortValueType.RPM, "RPM_NORMALIZED");
        public readonly PortDefinition GeneratedDamage = new(PortType.READONLY_OUT, PortValueType.DAMAGE, "GENERATED_DAMAGE");
        public readonly PortDefinition OverspeedSound = new(PortType.READONLY_OUT, PortValueType.STATE, "OVERSPEED_SOUND");

        public override SimComponent InstantiateImplementation()
        {
            return new RPMDamageCalculator(this);
        }
    }
}
