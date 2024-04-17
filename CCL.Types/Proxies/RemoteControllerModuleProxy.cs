using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies
{
    public class RemoteControllerModuleProxy : MonoBehaviour, IHasFuseIdFields
    {
        [FuseId]
        public string powerFuseId;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[] { new FuseIdField(this, nameof(powerFuseId), powerFuseId) };
    }
}
