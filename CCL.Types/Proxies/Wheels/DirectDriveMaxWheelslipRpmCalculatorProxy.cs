using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    public class DirectDriveMaxWheelslipRpmCalculatorProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortValueType.RPM, false)]
        public string engineRpmMaxPortId;
        [PortId(DVPortValueType.GENERIC, false)]
        public string gearRatioPortId;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(engineRpmMaxPortId), engineRpmMaxPortId, DVPortValueType.RPM),
            new PortIdField(this, nameof(gearRatioPortId), gearRatioPortId, DVPortValueType.GENERIC),
        };
    }
}
