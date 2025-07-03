using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    [AddComponentMenu("CCL/Proxies/Wheels/Direct Drive Max Wheelslip RPM Calculator Proxy")]
    public class DirectDriveMaxWheelslipRpmCalculatorProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortValueType.RPM, false)]
        public string engineRpmMaxPortId = string.Empty;
        [PortId(DVPortValueType.GENERIC, false)]
        public string gearRatioPortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(engineRpmMaxPortId), engineRpmMaxPortId, DVPortValueType.RPM),
            new PortIdField(this, nameof(gearRatioPortId), gearRatioPortId, DVPortValueType.GENERIC),
        };
    }
}
