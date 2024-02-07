using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public class LampControlProxy : MonoBehaviour
    {
        public IndicatorEmissionProxy lampInd;

        public LampState lampState;

        public AudioClip warningAudio;

        public AudioClip onStateBuzzingLoopAudio;

        public AudioClip blinkStateBuzzingLoopAudio;

        public enum LampState
        {
            Off,
            On,
            Blinking,
            None
        }
    }
}
