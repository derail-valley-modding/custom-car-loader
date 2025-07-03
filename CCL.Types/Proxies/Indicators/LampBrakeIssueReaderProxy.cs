using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    [AddComponentMenu("CCL/Proxies/Indicators/Lamp Brake Issue Reader Proxy")]
    public class LampBrakeIssueReaderProxy : MonoBehaviour, IHasFuseIdFields
    {
        [FuseId]
        public string lampPowerFuseId = string.Empty;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(lampPowerFuseId), lampPowerFuseId),
        };
    }
}
