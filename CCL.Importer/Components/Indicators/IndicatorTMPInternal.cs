using TMPro;

namespace CCL.Importer.Components.Indicators
{
    internal class IndicatorTMPInternal : IndicatorWithModeAndNamesInternal
    {
        public TMP_Text Text = null!;

        protected override void OnValueSet()
        {
            Text.text = GetDisplayText();
        }
    }
}
