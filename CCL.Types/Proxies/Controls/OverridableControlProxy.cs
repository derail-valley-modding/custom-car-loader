using CCL.Types.Proxies.Ports;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class OverridableControlProxy : MonoBehaviour
    {
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, true)]
        public string portId;

        public OverridableControlType ControlType;
    }
}
