using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    public abstract class WiperDriverProxy : MonoBehaviour
    {
        public WiperDriverProxy master = null!;
        public WiperProxy wiper = null!;
        public float speed;
        public float timeBetweenWipes;
    }
}
