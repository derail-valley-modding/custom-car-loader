using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    [AddComponentMenu("CCL/Proxies/Weather/Wiper Audio Proxy")]
    public class WiperAudioProxy : MonoBehaviour
    {
        public WiperControllerProxy wiperController = null!;
        public WiperDriverProxy driver = null!;
        public float[] pitchMultiplierValues = new float[0];
        public AnimationCurve slideVolumeCurve = null!;
        public AudioClip wetClip = null!;
        public AudioClip dryClip = null!;
        public AudioClip motorClip = null!;
        public AnimationCurve leftMove = null!;
        public AnimationCurve rightMove = null!;
        public AudioSource slideAudioSource = null!;
        public AudioSource motorAudioSource = null!;
        public float slideVolume;
        public AudioSource endAudio = null!;
        public AudioClip endClip = null!;
        public float endVolume;
        public float endPitchLeft;
        public float endPitchRight;
    }
}
