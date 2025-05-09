using CCL.Types.Proxies;
using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components
{
    [NotExposed]
    public class SimPortPlotter : MonoBehaviour
    {
        public int TickRate = 5;
        [PortId(null, null, false)]
        public List<string> PortIds = new List<string>();
        [PortReferenceId]
        public List<string> PortReferenceIds = new List<string>();
        public bool UseColours = true;
    }
}
