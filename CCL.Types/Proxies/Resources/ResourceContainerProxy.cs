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
        public float capacity = 100;
        public float defaultValue = 100;
        public ResourceContainerType type;

        public override IEnumerable<PortDefinition> ExposedPorts
        {
            get
            {
                var valueType = type.PortValueType();
                return new[]
                {
                    new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "CAPACITY"),
                    new PortDefinition(DVPortType.EXTERNAL_IN, valueType, "REFILL_EXT_IN"),
                    new PortDefinition(DVPortType.EXTERNAL_IN, valueType, "CONSUME_EXT_IN"),
                    new PortDefinition(DVPortType.READONLY_OUT, valueType, "AMOUNT"),
                    new PortDefinition(DVPortType.READONLY_OUT, valueType, "NORMALIZED"),
                };
            }
        }
    }
}
