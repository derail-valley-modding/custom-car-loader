using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    public class WiperDriverProxy : MonoBehaviour
    {
        public WiperDriverProxy master;
        public WiperProxy wiper;
        public float speed;
        public float timeBetweenWipes;
    }
}
