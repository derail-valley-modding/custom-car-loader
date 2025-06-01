using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    public class SimpleWiperAudio : MonoBehaviour
    {
        public enum WiperType
        {
            Electric = 0,
            Pneumatic = 100
        }

        public WiperType Type;
        public WiperControllerProxy wiperController = null!;
        public WiperDriverProxy driver = null!;
        public float[] pitchMultiplierValues = new float[0];
    }
}
