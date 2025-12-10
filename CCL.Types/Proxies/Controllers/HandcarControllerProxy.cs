using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class HandcarControllerProxy : MonoBehaviour, IHasPortIdFields
    {
        public Animator handlebarAnimator = null!;
        public Transform visualHandlebar = null!;
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.STATE, true)]
        public string directionPortId = string.Empty;
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.STATE, true)]
        public string currentPositionPortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(directionPortId), directionPortId, DVPortType.READONLY_OUT, DVPortValueType.STATE),
            new PortIdField(this, nameof(currentPositionPortId), currentPositionPortId, DVPortType.READONLY_OUT, DVPortValueType.STATE)
        };
    }
}
