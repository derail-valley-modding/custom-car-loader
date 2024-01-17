using AutoMapper;
using CCL.Types.Proxies.Indicators;
using DV.Simulation.Ports;
using System.ComponentModel.Composition;

namespace CCL.Importer.Proxies.Indicators
{
    public class IndicatorPortReaderReplacer : Profile
    {
        public IndicatorPortReaderReplacer()
        {
            CreateMap<IndicatorPortReaderProxy, IndicatorPortReader>().CacheAndProcessProxyAutomatically();
            CreateMap<IndicatorEmissionProxy, IndicatorEmission>().CacheAndProcessProxyAutomatically();
            CreateMap<IndicatorGaugeProxy, IndicatorGauge>().CacheAndProcessProxyAutomatically();
            CreateMap<IndicatorModelChangerProxy, IndicatorModelChanger>().CacheAndProcessProxyAutomatically();
            CreateMap<IndicatorScalerProxy, IndicatorScaler>().CacheAndProcessProxyAutomatically();
            CreateMap<IndicatorSliderProxy, IndicatorSlider>().CacheAndProcessProxyAutomatically();
        }
    }
}
