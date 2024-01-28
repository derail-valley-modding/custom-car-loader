using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public abstract class IndicatorBrakeReaderProxy : MonoBehaviour, IHasFuseIdFields
    {
        [FuseId]
        public string fuseId;

        public virtual IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(fuseId), fuseId),
        };
    }

    public class IndicatorBrakeCylinderReaderProxy : IndicatorBrakeReaderProxy { }

    public class IndicatorBrakePipeReaderProxy : IndicatorBrakeReaderProxy { }

    public class IndicatorBrakeReservoirReaderProxy : IndicatorBrakeReaderProxy { }
}
