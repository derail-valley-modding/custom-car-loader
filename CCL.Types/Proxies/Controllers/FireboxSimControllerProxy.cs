using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controllers
{
    public class FireboxSimControllerProxy : MonoBehaviour, IHasPortIdFields, IS060Defaults, IS282Defaults
    {
        public float coalConsumptionMultiplier = 1f;

        [PortId(null, null, false)]
        public string fireboxCapacityPortId;
        [PortId(null, null, false)]
        public string fireboxContentsPortId;
        [PortId(null, null, false)]
        public string fireboxDoorPortId;
        [PortId(null, null, false)]
        public string combustionRateNormalizedPortId;
        [PortId(null, null, false)]
        public string fireOnPortId;
        [PortId(null, null, false)]
        public string fireboxCoalControlPortId;
        [PortId(null, null, false)]
        public string fireboxIgnitionPortId;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(fireboxCapacityPortId), fireboxCapacityPortId, DVPortValueType.GENERIC),
            new PortIdField(this, nameof(fireboxContentsPortId), fireboxContentsPortId, DVPortValueType.GENERIC),
            new PortIdField(this, nameof(fireboxDoorPortId), fireboxDoorPortId, DVPortValueType.GENERIC),
            new PortIdField(this, nameof(combustionRateNormalizedPortId), combustionRateNormalizedPortId, DVPortValueType.GENERIC),
            new PortIdField(this, nameof(fireOnPortId), fireOnPortId, DVPortValueType.GENERIC),
            new PortIdField(this, nameof(fireboxCoalControlPortId), fireboxCoalControlPortId, DVPortValueType.GENERIC),
            new PortIdField(this, nameof(fireboxIgnitionPortId), fireboxIgnitionPortId, DVPortValueType.GENERIC)
        };

        #region Defaults

        public void ApplyS060Defaults()
        {
            coalConsumptionMultiplier = 2.0f;
        }

        public void ApplyS282Defaults()
        {
            coalConsumptionMultiplier = 4.0f;
        }

        #endregion
    }
}
