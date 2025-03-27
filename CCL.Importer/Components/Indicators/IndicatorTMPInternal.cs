using CCL.Types.Components.Indicators;
using TMPro;
using UnityEngine;

namespace CCL.Importer.Components.Indicators
{
    internal class IndicatorTMPInternal : Indicator
    {
        public TMP_Text Text = null!;
        public IndicatorTMP.Mode DisplayMode;
        public string Suffix = string.Empty;
        public string[] Names = new string[0];

        private int RoundedValue => Mathf.RoundToInt(Value);

        protected override void OnValueSet()
        {
            Text.text = DisplayMode switch
            {
                IndicatorTMP.Mode.Value => Value.ToString("F2") + Suffix,
                IndicatorTMP.Mode.RoundedValue => RoundedValue.ToString("F0") + Suffix,
                IndicatorTMP.Mode.Names => GetNameFromValue() + Suffix,
                _ => string.Empty,
            };
        }

        private string GetNameFromValue()
        {
            return Names[
                Mathf.RoundToInt(
                    NumberUtil.MapClamp(Value, minValue, maxValue, 0, Names.Length - 1))];
        }
    }
}
