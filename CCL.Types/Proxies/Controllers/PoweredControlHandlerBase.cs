using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public abstract class PoweredControlHandlerBase : MonoBehaviour, IHasPortIdFields, IHasFuseIdFields
    {
        [PortId(DVPortValueType.CONTROL, false)]
        public string controlId = string.Empty;
        [FuseId]
        public string powerFuseId = string.Empty;

        public virtual IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(controlId), controlId, DVPortValueType.CONTROL),
        };

        public virtual IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(powerFuseId), powerFuseId),
        };
    }
}
