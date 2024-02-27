using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class DeadTractionMotorsControllerProxy : MonoBehaviour, IHasPortIdFields, IHasFuseIdFields
    {
        [PortId(null, null, true)]
        public string overheatFuseOffPortId;
        [FuseId]
        public string tmFuseId;
        [Space]
        public Transform tmBlowAnchor;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(overheatFuseOffPortId), overheatFuseOffPortId, DVPortValueType.GENERIC),
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(tmFuseId), tmFuseId),
        };
    }
}
