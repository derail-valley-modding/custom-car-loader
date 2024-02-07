using AutoMapper;
using CCL.Types.Proxies.Simulation;
using DV.Simulation.Cars;
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
            CreateMap<TractionDefinitionProxy, TractionDefinition>()
                .AutoCacheAndMap()
                .AfterMap((_, dest) => AddDrivingForce(dest));

            CreateMap<TractionPortFeedersProxy, TractionPortsFeeder>().AutoCacheAndMap();
        }

        private static void AddDrivingForce(TractionDefinition traction)
        {
            var drivingForce = traction.gameObject.AddComponent<DrivingForce>();
            drivingForce.torqueGeneratedPortId = $"{traction.ID}.{traction.torqueIn.ID}";
        }
    }
}
