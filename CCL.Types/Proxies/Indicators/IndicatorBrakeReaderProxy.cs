using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public abstract class IndicatorBrakeReaderProxy : MonoBehaviour, IHasFuseIdFields
    {
        [FuseId]
        public string fuseId = string.Empty;

        public virtual IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(fuseId), fuseId),
        };
    }

    [AddComponentMenu("CCL/Proxies/Indicators/Indicator Brake Cylinder Reader Proxy")]
    public class IndicatorBrakeCylinderReaderProxy : IndicatorBrakeReaderProxy { }

    [AddComponentMenu("CCL/Proxies/Indicators/Indicator Brake Pipe Reader Proxy")]
    public class IndicatorBrakePipeReaderProxy : IndicatorBrakeReaderProxy { }

    [AddComponentMenu("CCL/Proxies/Indicators/Indicator Brake Reservoir Reader Proxy")]
    public class IndicatorBrakeReservoirReaderProxy : IndicatorBrakeReaderProxy { }
}
