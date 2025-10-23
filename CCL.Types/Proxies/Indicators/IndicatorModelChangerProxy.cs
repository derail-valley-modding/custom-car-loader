using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    [AddComponentMenu("CCL/Proxies/Indicators/Indicator Model Changer Proxy")]
    public class IndicatorModelChangerProxy : IndicatorProxy, ISelfValidation
    {
        [Tooltip("Ordered list of different models that indicator will switch on/off depending on the indicator value")]
        public GameObject[] indicatorModels = new GameObject[0];
        [Tooltip("Specific ordered low to high percentages, that tell us when the model switch will occur\n" +
            "Number of switchPercentages should always be indicatorModels.Count - 1, because we have implicit 0 percentage")]
        public float[] switchPercentage = new float[0];

        public SelfValidationResult Validate(out string message)
        {
            if (switchPercentage.Length != indicatorModels.Length - 1)
            {
                message = $"{nameof(switchPercentage)} should have 1 less item than {nameof(indicatorModels)}";
                return SelfValidationResult.Warning;
            }

            return this.Pass(out message);
        }
    }
}
