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
        [PortId]
        public List<string> PortIds = new List<string>();
        [PortReferenceId]
        public List<string> PortReferenceIds = new List<string>();
        public bool UseColours = true;
        [Tooltip("Adds a dummy port that outputs the generated force of the vehicle")]
        public bool AddDummyForcePort = true;
        [Tooltip("Adds a dummy port that outputs the generated power of the vehicle")]
        public bool AddDummyPowerPort = true;
    }
}
