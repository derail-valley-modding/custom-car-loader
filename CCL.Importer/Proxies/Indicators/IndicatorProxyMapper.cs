using CCL.Types.Proxies.Indicators;
using DV.Simulation.Ports;
using System.ComponentModel.Composition;

namespace CCL.Importer.Proxies.Indicators
{
    [Export(typeof(IProxyReplacer))]
    public class IndicatorPortReaderReplacer : ProxyReplacer<IndicatorPortReaderProxy, IndicatorPortReader> { }

    [Export(typeof(IProxyReplacer))]
    public class IndicatorEmissionReplacer: ProxyReplacer<IndicatorEmissionProxy, IndicatorEmission> { }

    [Export(typeof(IProxyReplacer))]
    public class IndicatorGaugeReplacer : ProxyReplacer<IndicatorGaugeProxy, IndicatorGauge> { }

    [Export(typeof(IProxyReplacer))]
    public class IndicatorModelChangerReplacer : ProxyReplacer<IndicatorModelChangerProxy, IndicatorModelChanger> { }

    [Export(typeof(IProxyReplacer))]
    public class IndicatorScalerReplacer : ProxyReplacer<IndicatorScalerProxy, IndicatorScaler> { }

    [Export(typeof(IProxyReplacer))]
    public class IndicatorSliderReplacer : ProxyReplacer<IndicatorSliderProxy, IndicatorSlider> { }
}
