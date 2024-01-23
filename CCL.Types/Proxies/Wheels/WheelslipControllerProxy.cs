using CCL.Types.Proxies.Ports;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    public class WheelslipControllerProxy : MonoBehaviour, IHasPortIdFields
    {
        public bool preventWheelslip;

        [PortId(DVPortValueType.GENERIC, false)]
        public string numberOfPoweredAxlesPortId;

        [PortId(DVPortValueType.STATE, false)]
        public string sandCoefPortId;

        [PortId(DVPortValueType.STATE, false)]
        public string engineBrakingActivePortId;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(numberOfPoweredAxlesPortId), numberOfPoweredAxlesPortId, DVPortValueType.GENERIC),
            new PortIdField(this, nameof(sandCoefPortId), sandCoefPortId, DVPortValueType.STATE),
            new PortIdField(this, nameof(engineBrakingActivePortId), engineBrakingActivePortId, DVPortValueType.STATE),
        };
    }
}
