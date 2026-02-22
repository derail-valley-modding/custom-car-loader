using CCL.Types.Proxies.Indicators;

namespace CCL.Types.Components.Indicators
{
    public abstract class IndicatorWithModeAndNames : IndicatorProxy
    {
        public IndicatorMode DisplayMode;
        [EnableIf(nameof(ShowDecimals))]
        public int Decimals = 2;
        public string Suffix = string.Empty;
        [EnableIf(nameof(ShowNames))]
        public string[] Names = new string[0];

        protected bool ShowDecimals => DisplayMode == IndicatorMode.Value;
        protected bool ShowNames => DisplayMode == IndicatorMode.Names;
    }
}
