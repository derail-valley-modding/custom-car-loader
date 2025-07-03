using CCL.Types.Proxies.Indicators;
using TMPro;
using UnityEngine;

namespace CCL.Types.Components.Indicators
{
    [AddComponentMenu("CCL/Components/Indicators/Indicator TMP")]
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
