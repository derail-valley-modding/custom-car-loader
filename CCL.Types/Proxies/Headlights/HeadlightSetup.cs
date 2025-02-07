using UnityEngine;

namespace CCL.Types.Proxies.Headlights
{
    public class HeadlightSetup : MonoBehaviour
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
        public HeadlightsSubControllerBaseProxy[] subControllers;
        public bool mainOffSetup;
    }
}
