using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    [AddComponentMenu("CCL/Proxies/Ports/Base Ports Overrider Proxy")]
    public class BasePortsOverriderProxy : MonoBehaviour, IHasPortIdFields
    {
        [Header("Steamer")]
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.WATER, false)]
        public string boilerSpecialRequestPortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, false)]
        public string oilingPointsSpecialRequestPortId = string.Empty;
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, false)]
        public string lubricatorSpecialRequestPortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(boilerSpecialRequestPortId), boilerSpecialRequestPortId, DVPortType.EXTERNAL_IN, DVPortValueType.WATER),
            new PortIdField(this, nameof(oilingPointsSpecialRequestPortId), oilingPointsSpecialRequestPortId, DVPortType.EXTERNAL_IN, DVPortValueType.STATE),
            new PortIdField(this, nameof(lubricatorSpecialRequestPortId), lubricatorSpecialRequestPortId, DVPortType.EXTERNAL_IN, DVPortValueType.STATE)
        };
    }
}
