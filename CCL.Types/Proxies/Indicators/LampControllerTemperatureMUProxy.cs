using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    [AddComponentMenu("CCL/Proxies/Indicators/Lamp Controller Temperature MU Proxy")]
    public class LampControllerTemperatureMUProxy : MonoBehaviour, IHasFuseIdFields
    {
        [FuseId]
        public string fuseId = string.Empty;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(fuseId), fuseId),
        };
    }
}
