using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Sim Data Display Sim Controller Proxy")]
    [NotExposed]
    public class SimDataDisplaySimControllerProxy : MonoBehaviour
    {
        public int dataQueueSize = 4000;
        public int sampleTickRate = 5;
        [PortId(null, null, false)]
        public List<string> portIdsToPlot = new List<string>();
        [PortReferenceId]
        public List<string> portReferenceIdsToPlot = new List<string>();
    }
}
