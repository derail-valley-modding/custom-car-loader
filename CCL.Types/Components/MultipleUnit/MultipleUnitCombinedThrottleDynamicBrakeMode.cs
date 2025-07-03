using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.MultipleUnit
{
    [AddComponentMenu("CCL/Components/Multiple Unit/Multiple Unit Combined Throttle Dynamic Brake Mode")]
    public class MultipleUnitCombinedThrottleDynamicBrakeMode :
        MultipleUnitExtraControl<MultipleUnitCombinedThrottleDynamicBrakeMode>, IHasPortIdFields
    {
        [PortId]
        public string ModePortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(ModePortId), ModePortId),
        };
    }
}
