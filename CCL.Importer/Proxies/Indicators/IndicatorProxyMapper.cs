using AutoMapper;
using CCL.Types.Proxies.Indicators;
using DV.Indicators;
using DV.Localization;
using DV.Simulation.Brake;
using DV.Simulation.Fuses;
using DV.Simulation.Lamps;
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
            CreateMap<IndicatorGaugeLaggingProxy, IndicatorGaugeLagging>().AutoCacheAndMap()
                .AfterMap(IndicatorGaugeLaggingAfter);
            CreateMap<IndicatorModelChangerProxy, IndicatorModelChanger>().AutoCacheAndMap();
            CreateMap<IndicatorScalerProxy, IndicatorScaler>().AutoCacheAndMap();
            CreateMap<IndicatorSliderProxy, IndicatorSlider>().AutoCacheAndMap();
            CreateMap<IndicatorShaderValueProxy, IndicatorShaderValue>().AutoCacheAndMap();

            CreateMap<LampPortReaderProxy, LampPortReader>().AutoCacheAndMap();
            CreateMap<LampFuseReaderProxy, LampFuseReader>().AutoCacheAndMap();
            CreateMap<LampBrakeIssueReaderProxy, LampBrakeLeaksAndHandbrakeStateReader>().AutoCacheAndMap();
            CreateMap<LampControlProxy, LampControl>().AutoCacheAndMap()
                .ForMember(d => d.lampInd, o => o.MapFrom(s => Mapper.GetFromCache(s.lampInd)))
                .ForMember(d => d.lampAudioMixerGroup, o => o.MapFrom(s => s.audioMixerGroup.ToInstance()));
            CreateMap<LampWheelSlipSlideReaderProxy, LampWheelSlipSlideReader>().AutoCacheAndMap();

            CreateMap<LabelLocalizer, Localize>();
        }

        private void IndicatorGaugeLaggingAfter(IndicatorGaugeLaggingProxy src, IndicatorGaugeLagging dest)
        {
            // Prevent disabling the component, as it is getting mapped twice for some reason.
            // At least until a better solution is found.
            if (src.EnabledState != null)
            {
                dest.enabled = src.EnabledState.Value;
            }
            else
            {
                src.EnabledState = src.enabled;
            }
        }
    }
}
