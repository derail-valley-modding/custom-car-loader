using CCL.Types.Proxies.Ports;
using UnityEngine;

namespace CCL.Types.Proxies
{
    public class MultipleUnitStateObserverProxy : MonoBehaviour
    {
        [Header("optional")]
        [PortId(DVPortValueType.TEMPERATURE, false)]
        public string temperaturePortId;
        [SerializeField]
        private float overheatStandardThreshold = 90f;
        [SerializeField]
        private float overheatCriticalThreshold = 105f;
    }
}
