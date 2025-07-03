using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    [AddComponentMenu("CCL/Proxies/Ports/Animator Port Reader Proxy")]
    public class AnimatorPortReaderProxy : MonoBehaviour, IHasPortIdFields
    {
        public enum UpdateType
        {
            SET_NORMALIZED_TIME,
            SET_PARAMETER
        }

        public UpdateType updateType;
        [PortId(null, null, false)]
        public string portId = string.Empty;
        [EnableIf(nameof(EnableParameter))]
        public string parameterName = string.Empty;

        [Header("Value modifiers")]
        public float valueMultiplier = 1f;
        public float valueOffset;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(portId), portId),
        };

        private bool EnableParameter() => updateType == UpdateType.SET_PARAMETER;
    }
}
