using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Resources
{
    public class ResourceContainerProxy : MonoBehaviour
    {
        public string ID;
        public float capacity;
        public float defaultValue;
        public ResourceContainerType type;
    }
}
