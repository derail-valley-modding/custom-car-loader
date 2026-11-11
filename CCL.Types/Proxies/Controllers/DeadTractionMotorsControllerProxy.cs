using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    [AddComponentMenu("CCL/Proxies/Controllers/Dead Traction Motors Controller Proxy")]
    public class DeadTractionMotorsControllerProxy : MonoBehaviour, IHasPortIdFields, IHasFuseIdFields, ISelfValidation
    {
        [PortId(null, null, true)]
        public string overheatFuseOffPortId = string.Empty;
        [FuseId]
        public string tmFuseId = string.Empty;
        [Space]
        public Transform tmBlowAnchor = null!;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(overheatFuseOffPortId), overheatFuseOffPortId),
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(tmFuseId), tmFuseId),
        };

        public SelfValidationResult Validate(out string message, out string? highlight)
        {
            if (tmBlowAnchor == null)
            {
                return this.FailForNull(nameof(tmBlowAnchor), out message, out highlight);
            }

            return this.Pass(out  message, out highlight);
        }
    }
}
