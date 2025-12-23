using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;
using static CCL.Types.Components.Simulation.FuseLogicDefinition;

namespace CCL.Importer.Components.Simulation
{
    internal class FuseLogicDefinitionInternal : SimComponentDefinition
    {
        public string FuseA = string.Empty;
        public string FuseB = string.Empty;

        public LogicType Logic = LogicType.AND;

        public FuseDefinition OutputFuse = new("OUT", false);

        public override SimComponent InstantiateImplementation()
        {
            return new FuseLogic(this);
        }
    }
}
