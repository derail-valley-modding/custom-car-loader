using CCL.Types.Proxies.Indicators;
using TMPro;

namespace CCL.Types.Components.Indicators
{
    public class IndicatorTMP : IndicatorProxy
    {
        public enum Mode
        {
            Value,
            RoundedValue,
            Names
        }

        public TMP_Text Text = null!;
        public Mode DisplayMode;
        public string Suffix = string.Empty;
        public string[] Names = new string[0];
    }
}
