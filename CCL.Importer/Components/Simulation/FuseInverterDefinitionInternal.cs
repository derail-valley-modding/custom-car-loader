using CCL.Importer.Implementations;
using LocoSim.Definitions;
using LocoSim.Implementations;
using System;

namespace CCL.Importer.Components.Simulation
{
    internal class FuseInverterDefinitionInternal : SimComponentDefinition
    {
        public string[] SourceFuses = new string[0];
        public string[] InvertedFuses = new string[0];

        public override SimComponent InstantiateImplementation()
        {
            return new FuseInverter(this);
        }
    }
}
