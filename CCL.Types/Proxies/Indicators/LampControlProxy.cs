using CCL.Types.Proxies.Audio;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    [AddComponentMenu("CCL/Proxies/Indicators/Lamp Control Proxy")]
    public class LampControlProxy : MonoBehaviour
    {
        public IndicatorEmissionProxy lampInd = null!;
        public LampState lampState;
        public AudioClip warningAudio = null!;
        public AudioClip onStateBuzzingLoopAudio = null!;
        public AudioClip blinkStateBuzzingLoopAudio = null!;
        public DVAudioMixerGroup audioMixerGroup = DVAudioMixerGroup.Cab;

        public enum LampState
        {
            Off,
            On,
            Blinking,
            None
        }
    }
}
