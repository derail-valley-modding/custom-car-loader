using AutoMapper;
using CCL.Types.Proxies.Simulation;
using LocoSim.Definitions;

namespace CCL.Importer.Proxies.Simulation
{
    public class SimulationReplacer : Profile
    {
        public SimulationReplacer()
        {
            CreateMap<SanderDefinitionProxy, SanderDefinition>().AutoCacheAndMap();

            CreateMap<TransmissionFixedGearDefinitionProxy, TransmissionFixedGearDefinition>().AutoCacheAndMap();
            CreateMap<TractionDefinitionProxy, TractionDefinition>().AutoCacheAndMap();
        }
    }
}
