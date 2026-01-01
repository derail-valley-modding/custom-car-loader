using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    [AddComponentMenu("CCL/Proxies/Controllers/Gear Shifter Proxy")]
    public class GearShifterProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, true)]
        public string currentGearRatioPortId = string.Empty;
        public bool isGearboxA;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(currentGearRatioPortId), currentGearRatioPortId, DVPortType.READONLY_OUT, DVPortValueType.GENERIC)
        };
    }
}
