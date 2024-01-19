using AutoMapper;
using CCL.Types.Proxies.Simulation.Diesel;
using LocoSim.Definitions;

namespace CCL.Importer.Proxies.Simulation
{
    internal class DieselSimulationReplacer : Profile
    {
        public DieselSimulationReplacer() 
        {
            CreateMap<ThrottleNotchPowerProxy, ThrottleCustomPowerConversionDefinition>().AutoCacheAndMap();
            CreateMap<TractionMotorDefinitionProxy, TractionMotorDefinition>().AutoCacheAndMap();
            CreateMap<DieselElectricGensetProxy, DieselEnginePowerSourceDefinition>().AutoCacheAndMap();

            // heat
            CreateMap<PassiveCoolerDefinitionProxy, PassiveCoolerDefinition>().AutoCacheAndMap();
            CreateMap<DirectionalCoolerDefinitionProxy, DirectionalMovementCoolerDefinition>().AutoCacheAndMap();
            CreateMap<HeatReservoirDefinitionProxy, HeatReservoirDefinition>().AutoCacheAndMap();

            // air
            CreateMap<CompressorDieselEngineDefinitionProxy, CompressorDieselEngineDefinition>().AutoCacheAndMap();
        }
    }
}
