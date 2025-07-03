using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Controllers
{
    [AddComponentMenu("CCL/Components/Controllers/Resourcec Sharer Controller")]
    public class ResourceSharerController : MonoBehaviour, IHasPortIdFields
    {
        public ResourceContainerType Type = ResourceContainerType.Water;
        [Min(0), Tooltip("Amount of resources transfered per second at maximum flow rate")]
        public float MaxTransfer = 500.0f;
        [Min(0), Tooltip("Amount of resources transfered per second at minimum flow rate")]
        public float MinTransfer = 10.0f;
        public bool AllowSharingToFront = true;
        public bool AllowSharingToRear = true;

        [PortId(DVPortType.READONLY_OUT)]
        public string CapacityPortId = string.Empty;
        [PortId(DVPortType.READONLY_OUT)]
        public string AmountPortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN)]
        public string ConsumePortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN)]
        public string RefillPortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields
        {
            get
            {
                var valueType = Type.PortValueType();
                return new[]
                {
                    new PortIdField(this, nameof(CapacityPortId), CapacityPortId, DVPortType.READONLY_OUT, valueType),
                    new PortIdField(this, nameof(AmountPortId), AmountPortId, DVPortType.READONLY_OUT, valueType),
                    new PortIdField(this, nameof(ConsumePortId), ConsumePortId, DVPortType.EXTERNAL_IN, valueType),
                    new PortIdField(this, nameof(RefillPortId), RefillPortId, DVPortType.EXTERNAL_IN, valueType)
                };
            }
        }

        private void Reset()
        {
            if (!TryGetComponent(out ResourceContainerProxy container)) return;

            Type = container.type;
            CapacityPortId = container.GetFullPortId("CAPACITY");
            AmountPortId = container.GetFullPortId("AMOUNT");
            ConsumePortId = container.GetFullPortId("CONSUME_EXT_IN");
            RefillPortId = container.GetFullPortId("REFILL_EXT_IN");
        }
    }
}
