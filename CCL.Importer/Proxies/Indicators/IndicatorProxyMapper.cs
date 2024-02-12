using AutoMapper;
using CCL.Types.Proxies.Indicators;
using DV.Indicators;
using DV.Localization;
using DV.Simulation.Brake;
using DV.Simulation.Fuses;
using DV.Simulation.Ports;

namespace CCL.Importer.Proxies.Indicators
{
    public class IndicatorPortReaderReplacer : Profile
    {
        public IndicatorPortReaderReplacer()
        {
            CreateMap<IndicatorPortReaderProxy, IndicatorPortReader>().AutoCacheAndMap();
            CreateMap<IndicatorBrakeCylinderReaderProxy, IndicatorBrakeCylinderReader>().AutoCacheAndMap();
            CreateMap<IndicatorBrakePipeReaderProxy, IndicatorBrakePipeReader>().AutoCacheAndMap();
            CreateMap<IndicatorBrakeReservoirReaderProxy, IndicatorMainResReader>().AutoCacheAndMap();

            CreateMap<IndicatorEmissionProxy, IndicatorEmission>().AutoCacheAndMap();
            CreateMap<IndicatorGaugeProxy, IndicatorGauge>().AutoCacheAndMap();
            CreateMap<IndicatorGaugeLaggingProxy, IndicatorGaugeLagging>().AutoCacheAndMap();
            CreateMap<IndicatorModelChangerProxy, IndicatorModelChanger>().AutoCacheAndMap();
            CreateMap<IndicatorScalerProxy, IndicatorScaler>().AutoCacheAndMap();
            CreateMap<IndicatorSliderProxy, IndicatorSlider>().AutoCacheAndMap();

            CreateMap<LampPortReaderProxy, LampPortReader>().AutoCacheAndMap();
            CreateMap<LampFuseReaderProxy, LampFuseReader>().AutoCacheAndMap();
            CreateMap<LampBrakeIssueReaderProxy, LampBrakeLeaksAndHandbrakeStateReader>().AutoCacheAndMap();
            CreateMap<LampControlProxy, LampControl>().AutoCacheAndMap()
                .WithCachedMember(d => d.lampInd);

            CreateMap<LabelLocalizer, Localize>().AutoCacheAndMap();
        }
    }
}
