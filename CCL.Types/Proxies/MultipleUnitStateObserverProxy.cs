using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Multiple Unit State Observer Proxy")]
    public class MultipleUnitStateObserverProxy : MonoBehaviour, IHasPortIdFields
    {
        [Header("optional")]
        [PortId(DVPortValueType.TEMPERATURE, false, false)]
        public string temperaturePortId = string.Empty;
        [SerializeField]
        private float overheatStandardThreshold = 90f;
        [SerializeField]
        private float overheatCriticalThreshold = 105f;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(temperaturePortId), temperaturePortId, DVPortValueType.TEMPERATURE, false)
        };
    }
}
