using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Components.MultipleUnit
{
    public class MultipleUnitCombinedThrottleDynamicBrakeMode :
        MultipleUnitExtraControl<MultipleUnitCombinedThrottleDynamicBrakeMode>, IHasPortIdFields
    {
        [PortId]
        public string ModePortId = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(ModePortId), ModePortId, DVPortType.READONLY_OUT, DVPortValueType.STATE),
        };
    }
}
