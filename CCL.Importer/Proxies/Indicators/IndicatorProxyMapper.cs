using CCL.Types.Proxies.Indicators;
using DV.Simulation.Ports;
using System.ComponentModel.Composition;

namespace CCL.Importer.Proxies.Indicators
{
    [ProxyMap(typeof(IndicatorPortReaderProxy), typeof(IndicatorPortReader))]
    [ProxyMap(typeof(IndicatorEmissionProxy), typeof(IndicatorEmission))]
    [ProxyMap(typeof(IndicatorGaugeProxy), typeof(IndicatorGauge))]
    [ProxyMap(typeof(IndicatorModelChangerProxy), typeof(IndicatorModelChanger))]
    [ProxyMap(typeof(IndicatorScalerProxy), typeof(IndicatorScaler))]
    [ProxyMap(typeof(IndicatorSliderProxy), typeof(IndicatorSlider))]
    public class IndicatorPortReaderReplacer : ProxyReplacer { }
}
