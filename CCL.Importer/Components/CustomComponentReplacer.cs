using AutoMapper;
using CCL.Importer.Components.Headlights;
using CCL.Importer.Components.Indicators;
using CCL.Types.Components;
using CCL.Types.Components.Headlights;
using CCL.Types.Components.Indicators;

namespace CCL.Importer.Components
{
    internal class CustomComponentReplacer : Profile
    {
        public CustomComponentReplacer()
        {
            MapCoupling();
            MapHeadlights();
            MapIndicators();
        }

        private void MapCoupling()
        {
            CreateMap<CarAutoCoupler, CarAutoCouplerInternal>().AutoCacheAndMap();
            CreateMap<RigidCoupler, RigidCouplerInternal>().AutoCacheAndMap();
            CreateMap<VirtualHandbrakeOverrider, VirtualHandbrakeOverriderInternal>().AutoCacheAndMap();
            CreateMap<DuplicateHandbrakeOverrider, DuplicateHandbrakeOverriderInternal>().AutoCacheAndMap();
        }

        private void MapHeadlights()
        {
            CreateMap<FrontConnectedDualCarAutomaticHeadlightsController, FrontConnectedDualCarAutomaticHeadlightsControllerInternal>().AutoCacheAndMap();
            CreateMap<FrontAndRearConnectedTriCarAutomaticHeadlightsController, FrontAndRearConnectedTriCarAutomaticHeadlightsControllerInternal>().AutoCacheAndMap();
        }

        private void MapIndicators()
        {
            CreateMap<IndicatorShaderCustomValue, IndicatorShaderCustomValueInternal>().AutoCacheAndMap();
        }
    }
}
