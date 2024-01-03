using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public abstract class SimComponentDefinitionProxy : MonoBehaviour
    {
        public string ID;

        public virtual IEnumerable<PortDefinition> ExposedPorts => Enumerable.Empty<PortDefinition>();
    }
}