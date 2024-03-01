using AutoMapper;
using CCL.Types.Proxies.Simulation.Electric;
using LocoSim.Definitions;

namespace CCL.Importer.Proxies.Simulation
{
    internal class ElectricSimulationReplacer : Profile
    {
        public ElectricSimulationReplacer()
        {
            CreateMap<TractionMotorSetDefinitionProxy, TractionMotorSetDefinition>().AutoCacheAndMap()
                .ForMember(d => d.poweredWheelsManager, o => o.MapFrom(s => Mapper.GetFromCache(s.poweredWheelsManager)));
            CreateMap<TractionMotorSetDefinitionProxy.ElectricalConfigurationDefinition, TractionMotorSetDefinition.ElectricalConfigurationDefinition>();
            CreateMap<TractionMotorSetDefinitionProxy.MotorGroupDefinition, TractionMotorSetDefinition.MotorGroupDefinition>();
            CreateMap<TractionMotorSetDefinitionProxy.TransitionDefinition, TractionMotorSetDefinition.TransitionDefinition>();

            CreateMap<TractionGeneratorDefinitionProxy, TractionGeneratorDefinition>().AutoCacheAndMap();

            CreateMap<BatteryDefinitionProxy, BatteryDefinition>().AutoCacheAndMap();
        }
    }
}
