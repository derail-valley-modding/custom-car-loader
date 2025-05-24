using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Controllers
{
    public class CoalPileSimControllerProxy : MonoBehaviourWithVehicleDefaults, IHasPortIdFields, IS060Defaults, IS282Defaults
    {
        public float coalChunkMass;
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.COAL, true)]
        public string coalAvailablePortId = string.Empty;
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.COAL, true)]
        public string coalCapacityPortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.COAL, true)]
        public string coalConsumePortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(coalAvailablePortId), coalAvailablePortId, DVPortType.READONLY_OUT, DVPortValueType.COAL),
            new PortIdField(this, nameof(coalCapacityPortId), coalCapacityPortId, DVPortType.READONLY_OUT, DVPortValueType.COAL),
            new PortIdField(this, nameof(coalConsumePortId), coalConsumePortId, DVPortType.EXTERNAL_IN, DVPortValueType.COAL),
        };

        public void ApplyS060Defaults()
        {
            coalChunkMass = 9.0f;
        }

        public void ApplyS282Defaults()
        {
            coalChunkMass = 48.0f;
        }
    }
}
