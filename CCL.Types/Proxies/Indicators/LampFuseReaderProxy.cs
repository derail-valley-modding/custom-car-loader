using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    [AddComponentMenu("CCL/Proxies/Indicators/Lamp Fuse Reader Proxy")]
    public class LampFuseReaderProxy : MonoBehaviour, IHasFuseIdFields
    {
        public enum Mode
        {
            ON_WHEN_FUSE_ON,
            BLINK_WHEN_FUSE_ON
        }

        public Mode mode;

        [FuseId(true)]
        public string fuseId = string.Empty;

        [Header("Optional")]
        [FuseId]
        public string powerFuseId = string.Empty;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(fuseId), fuseId, true),
            new FuseIdField(this, nameof(powerFuseId), powerFuseId),
        };
    }
}
