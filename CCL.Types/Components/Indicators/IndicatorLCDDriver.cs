using CCL.Types.Proxies;
using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Indicators
{
    [AddComponentMenu("CCL/Components/Indicators/Indicator LCD Driver")]
    public class IndicatorLCDDriver : IndicatorWithModeAndNames, IHasFuseIdFields
    {
        public LCDDriverProxy LCD = null!;
        public bool PadLeft = true;
        [Header("Optional")]
        [FuseId]
        public string FuseId = string.Empty;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(FuseId), FuseId)
        };
    }
}
