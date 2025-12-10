using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Interactable Fuse Feeder Proxy")]
    public class InteractableFuseFeederProxy : MonoBehaviour, IHasFuseIdFields
    {
        [FuseId(true)]
        public string fuseId = string.Empty;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(fuseId), fuseId, true),
        };
    }
}
