using CCL.Types.Proxies.Indicators;
using DV.Indicators;
using DV.Simulation.Ports;
using System.ComponentModel.Composition;

namespace CCL.Importer.Proxies.Indicators
{
    [ProxyMap(typeof(IndicatorPortReaderProxy), typeof(IndicatorPortReader))]
    [ProxyMap(typeof(IndicatorBrakeCylinderReaderProxy), typeof(IndicatorBrakeCylinderReader))]
    [ProxyMap(typeof(IndicatorBrakePipeReaderProxy), typeof(IndicatorBrakePipeReader))]
    [ProxyMap(typeof(IndicatorBrakeReservoirReaderProxy), typeof(IndicatorMainResReader))]

    [ProxyMap(typeof(IndicatorEmissionProxy), typeof(IndicatorEmission))]
    [ProxyMap(typeof(IndicatorGaugeProxy), typeof(IndicatorGauge))]
    [ProxyMap(typeof(IndicatorModelChangerProxy), typeof(IndicatorModelChanger))]
    [ProxyMap(typeof(IndicatorScalerProxy), typeof(IndicatorScaler))]
    [ProxyMap(typeof(IndicatorSliderProxy), typeof(IndicatorSlider))]
    public class IndicatorPortReaderReplacer : ProxyReplacer { }
}
