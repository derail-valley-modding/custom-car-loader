using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    public class WiperControllerProxy : MonoBehaviour
    {
        public float[] speeds = new float[]
        {
            0f,
            1f,
            1f,
            2f
        };

        public float[] timeBetweenWipes = new float[0];
        public WiperDriverProxy[] wiperDrivers = new WiperDriverProxy[0];
        public int speedIndex = 0;
    }
}
