using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies
{
    public class MultipleUnitStateObserverProxy : MonoBehaviour, IHasPortIdFields
    {
        [Header("optional")]
        [PortId(DVPortValueType.TEMPERATURE, false)]
        public string temperaturePortId;
        [SerializeField]
        private float overheatStandardThreshold = 90f;
        [SerializeField]
        private float overheatCriticalThreshold = 105f;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[] {
            new PortIdField(this, nameof(temperaturePortId), temperaturePortId, DVPortValueType.TEMPERATURE)
        };
    }
}
