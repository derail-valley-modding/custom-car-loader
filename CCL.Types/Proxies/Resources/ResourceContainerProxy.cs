using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCL.Types.Proxies.Ports;
using UnityEngine;

namespace CCL.Types.Proxies.Resources
{
    public class ResourceContainerProxy : SimComponentDefinitionProxy
    {
        public float capacity;
        public float defaultValue;
        public ResourceContainerType type;
    }
}
