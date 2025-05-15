using CCL.Types;
using CCL.Types.Proxies.Indicators;
using UnityEngine;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class IndicatorValidator : LiveryValidator
    {
        public override string TestName => "Indicators";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            var result = Pass();

            foreach (var indicator in livery.AllPrefabs.GetComponentsInChildren<IndicatorProxy>())
            {
                switch (indicator)
                {
                    case IndicatorGaugeProxy gauge:
                        if (gauge.needle == null)
                        {
                            result.Fail("IndicatorGauge does not have needle set.", indicator);
                        }
                        break;
                    case IndicatorEmissionProxy emission:
                        if (emission.renderers.Length == 0 && emission.GetComponentsInChildren<Renderer>().Length == 0)
                        {
                            result.Fail("IndicatorEmission does not any renderers assigned.", indicator);
                            continue;
                        }
                        foreach (var renderer in emission.renderers)
                        {
                            if (renderer == null)
                            {
                                result.Fail("IndicatorEmission: renderer is null.", indicator);
                                break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return result;
        }
    }
}
