using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Headlights
{
    [AddComponentMenu("CCL/Proxies/Headlights/Headlight Setup Proxy")]
    [NotProxied]
    public class HeadlightSetup : MonoBehaviour, ISelfValidation
    {
        public enum HeadlightSetting
        {
            Off,
            HeadlightSetting01,
            HeadlightSetting02,
            HeadlightSetting03,
            HeadlightSetting04,
            HeadlightSetting05,
            HeadlightSetting06,
            HeadlightSetting07,
            HeadlightSetting08,
            HeadlightSetting09,
            HeadlightSetting10,
            HeadlightSetting11,
            HeadlightSetting12,
            HeadlightSetting13,
            HeadlightSetting14,
            HeadlightSetting15,
            HeadlightSetting16
        }

        public HeadlightSetting setting;
        public HeadlightsSubControllerBaseProxy[] subControllers = new HeadlightsSubControllerBaseProxy[0];
        public bool mainOffSetup;

        public SelfValidationResult Validate(out string message)
        {
            if (subControllers.Any(x => x == null))
            {
                return this.FailForNullEntries(nameof(subControllers), out message);
            }

            return this.Pass(out message);
        }
    }
}
