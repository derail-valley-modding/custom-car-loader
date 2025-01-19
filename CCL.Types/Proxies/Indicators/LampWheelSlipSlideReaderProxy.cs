using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public class LampWheelSlipSlideReaderProxy : MonoBehaviour, IHasFuseIdFields
    {
        public enum WheelslipDetectionMode
        {
            Individual,
            MultipleUnit
        }

        public WheelslipDetectionMode wheelslipDetectionMode;
        [FuseId]
        public string fuseId;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(fuseId), fuseId),
        };
    }
}
