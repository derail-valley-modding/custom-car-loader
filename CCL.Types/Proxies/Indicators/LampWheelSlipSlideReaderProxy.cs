using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    [AddComponentMenu("CCL/Proxies/Indicators/Lamp Wheel Slip/Slide Reader Proxy")]
    public class LampWheelSlipSlideReaderProxy : MonoBehaviour, IHasFuseIdFields
    {
        public enum WheelslipDetectionMode
        {
            Individual,
            MultipleUnit
        }

        public WheelslipDetectionMode wheelslipDetectionMode;
        [FuseId]
        public string fuseId = string.Empty;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(fuseId), fuseId),
        };
    }
}
