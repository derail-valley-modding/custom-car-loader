using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class OverridableControlProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, true)]
        public string portId;
        public OverridableControlType ControlType;
        [Header("optional")]
        public ControlBlockerProxy controlBlocker;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[] 
        {
            new PortIdField(this, nameof(portId), portId, DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL)
        };

        public void OnValidate()
        {
            if (controlBlocker == null)
            {
                controlBlocker = GetComponent<ControlBlockerProxy>();
            }
        }
    }

    public class HornControlProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, true)]
        public string portId;
        public bool neutralAt0;
        [Header("optional")]
        public ControlBlockerProxy controlBlocker;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(portId), portId, DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL)
        };

        public void OnValidate()
        {
            if (controlBlocker == null)
            {
                controlBlocker = GetComponent<ControlBlockerProxy>();
            }
        }
    }
}
