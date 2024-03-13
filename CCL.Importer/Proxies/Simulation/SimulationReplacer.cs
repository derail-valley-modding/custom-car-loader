using AutoMapper;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Electric;
using DV.Simulation.Cars;
using DV.Simulation.Ports;
using LocoSim.Definitions;
using LocoSim.DVExtensions.Slugs;

namespace CCL.Importer.Proxies.Simulation
{
    public class SimulationReplacer : Profile
    {
        public SimulationReplacer()
        {
            CreateMap<SanderDefinitionProxy, SanderDefinition>().AutoCacheAndMap();

            // heat
            CreateMap<PassiveCoolerDefinitionProxy, PassiveCoolerDefinition>().AutoCacheAndMap();
            CreateMap<DirectionalCoolerDefinitionProxy, DirectionalMovementCoolerDefinition>().AutoCacheAndMap();
            CreateMap<HeatReservoirDefinitionProxy, HeatReservoirDefinition>().AutoCacheAndMap();

            CreateMap<SmoothTransmissionDefinitionProxy, SmoothTransmissionDefinition>().AutoCacheAndMap();
            CreateMap<TransmissionFixedGearDefinitionProxy, TransmissionFixedGearDefinition>().AutoCacheAndMap();
            CreateMap<TractionDefinitionProxy, TractionDefinition>()
                .AutoCacheAndMap()
                .AfterMap((_, dest) => AddDrivingForce(dest));

            CreateMap<TractionPortFeedersProxy, TractionPortsFeeder>().AutoCacheAndMap();

            CreateMap<SlugModuleProxy, SlugModule>().AutoCacheAndMap();
            CreateMap<SlugsPowerCalculatorDefinitionProxy, SlugsPowerCalculatorDefinition>().AutoCacheAndMap();
            CreateMap<SlugsPowerProviderModuleProxy, SlugsPowerProviderModule>().AutoCacheAndMap();
        }

        private static void AddDrivingForce(TractionDefinition traction)
        {
            var drivingForce = traction.gameObject.AddComponent<DrivingForce>();
            drivingForce.torqueGeneratedPortId = $"{traction.ID}.{traction.torqueIn.ID}";
        }
    }
}
