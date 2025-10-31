using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Indicators
{
    [AddComponentMenu("CCL/Components/Indicators/Indicator Handbrake Reader")]
    public class IndicatorHandbrakeReader : MonoBehaviour, IHasFuseIdFields
    {
        [FuseId]
        public string FuseId = string.Empty;
        
        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(FuseId), FuseId)
        };
    }
}
