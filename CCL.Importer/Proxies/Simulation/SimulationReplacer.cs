using AutoMapper;
using CCL.Types.Proxies.Simulation;
using DV.Simulation.Ports;
using LocoSim.Definitions;

namespace CCL.Importer.Proxies.Simulation
{
    public class SimulationReplacer : Profile
    {
        public SimulationReplacer()
        {
            CreateMap<SanderDefinitionProxy, SanderDefinition>().AutoCacheAndMap();

            CreateMap<SmoothTransmissionDefinitionProxy, SmoothTransmissionDefinition>().AutoCacheAndMap();
            CreateMap<TransmissionFixedGearDefinitionProxy, TransmissionFixedGearDefinition>().AutoCacheAndMap();
            CreateMap<TractionDefinitionProxy, TractionDefinition>().AutoCacheAndMap();
            CreateMap<TractionPortFeedersProxy, TractionPortsFeeder>().AutoCacheAndMap();
        }
    }
}
