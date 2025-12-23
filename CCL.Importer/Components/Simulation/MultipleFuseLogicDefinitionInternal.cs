using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;

using static CCL.Types.Components.Simulation.FuseLogicDefinition;

namespace CCL.Importer.Components.Simulation
{
    internal class MultipleFuseLogicDefinitionInternal : SimComponentDefinition
    {
        public string[] Fuses = new string[0];
        public LogicType Logic = LogicType.AND;
        public FuseDefinition OutputFuse = new("OUT", false);

        public override SimComponent InstantiateImplementation()
        {
            return new MultipleFuseLogic(this);
        }
    }
}
