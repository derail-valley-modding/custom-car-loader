using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    [AddComponentMenu("CCL/Proxies/Controllers/Compressor Sim Controller Proxy")]
    public class CompressorSimControllerProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, true)]
        public string activationSignalExtInPortId = string.Empty;
        [PortId(DVPortValueType.STATE, true)]
        public string productionRateOutPortId = string.Empty;
        [PortId(DVPortValueType.GENERIC, true)]
        public string mainReservoirVolumePortId = string.Empty;
        [PortId(DVPortValueType.GENERIC, true)]
        public string activationPressureThresholdPortId = string.Empty;

        [Header("optional")]
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, true)]
        public string compressorHealthStatePortId = string.Empty;
        [PortId(DVPortValueType.PRESSURE, true)]
        public string mainResPressureNormalizedPortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(activationSignalExtInPortId), activationSignalExtInPortId, DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL),
            new PortIdField(this, nameof(productionRateOutPortId), productionRateOutPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(mainReservoirVolumePortId), mainReservoirVolumePortId, DVPortValueType.GENERIC),
            new PortIdField(this, nameof(activationPressureThresholdPortId), activationPressureThresholdPortId, DVPortValueType.GENERIC),
            new PortIdField(this, nameof(compressorHealthStatePortId), compressorHealthStatePortId, DVPortType.EXTERNAL_IN, DVPortValueType.STATE),
            new PortIdField(this, nameof(mainResPressureNormalizedPortId), mainResPressureNormalizedPortId, DVPortValueType.PRESSURE),
        };
    }
}
