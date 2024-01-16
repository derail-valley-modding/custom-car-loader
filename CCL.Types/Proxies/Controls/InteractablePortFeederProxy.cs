using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class InteractablePortFeederProxy : MonoBehaviour
    {
        // This is an EXT_IN port which controls something, and is not required to be on the same prefab (but is on the same traincar
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, false)]
        public string portId;
    }
}
