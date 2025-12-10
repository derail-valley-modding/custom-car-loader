using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Overridable Control Proxy")]
    public class OverridableControlProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, true)]
        public string portId = string.Empty;
        public OverridableControlType ControlType;
        [Header("Optional")]
        public ControlBlockerProxy controlBlocker = null!;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[] 
        {
            new PortIdField(this, nameof(portId), portId, DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL)
        };

        private void Reset()
        {
            var externalControl = GetComponent<ExternalControlDefinitionProxy>();

            if (externalControl != null)
            {
                portId = externalControl.GetFullPortId("EXT_IN");
            }
        }

        public void OnValidate()
        {
            if (controlBlocker == null)
            {
                controlBlocker = GetComponent<ControlBlockerProxy>();
            }
        }
    }

    [AddComponentMenu("CCL/Proxies/Controls/Horn Control Proxy")]
    public class HornControlProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, true)]
        public string portId = string.Empty;
        [Header("optional")]
        public ControlBlockerProxy controlBlocker = null!;
        public bool neutralAt0;

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

    [AddComponentMenu("CCL/Proxies/Controls/Power Off Control Proxy")]
    public class PowerOffControlProxy : MonoBehaviour, IHasPortIdFields
    {
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, true)]
        public string portId = string.Empty;
        [Header("optional")]
        public ControlBlockerProxy controlBlocker = null!;
        public bool signalClearedBySim;

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
