using CCL.Types.Components.Indicators;
using UnityEngine;

namespace CCL.Importer.Components.Indicators
{
    internal abstract class IndicatorWithModeAndNamesInternal : Indicator
    {
        public IndicatorMode DisplayMode;
        public int Decimals = 2;
        public string Suffix = string.Empty;
        public string[] Names = new string[0];

        private string? _format;

        protected int RoundedValue => Mathf.RoundToInt(Value);
        protected string Format => Extensions.GetCached(ref _format, () => $"F{Decimals}");

        protected string GetDisplayText()
        {
            return DisplayMode switch
            {
                IndicatorMode.Value => Value.ToString(Format) + Suffix,
                IndicatorMode.RoundedValue => RoundedValue.ToString("F0") + Suffix,
                IndicatorMode.Names => GetNameFromValue() + Suffix,
                _ => string.Empty,
            };
        }

        protected string GetNameFromValue()
        {
            return Names[Mathf.RoundToInt(NumberUtil.MapClamp(Value, minValue, maxValue, 0, Names.Length - 1))];
        }
    }
}
