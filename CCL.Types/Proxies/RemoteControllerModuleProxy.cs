using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Remote Controller Module Proxy")]
    public class RemoteControllerModuleProxy : MonoBehaviour, IHasFuseIdFields
    {
        [FuseId]
        public string powerFuseId = string.Empty;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(powerFuseId), powerFuseId)
        };
    }
}
