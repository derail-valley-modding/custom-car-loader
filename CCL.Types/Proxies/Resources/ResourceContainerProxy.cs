using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCL.Types.Proxies.Ports;
using UnityEngine;

namespace CCL.Types.Proxies.Resources
{
    public class ResourceContainerProxy : SimComponentDefinitionProxy, IDM3Defaults, IDH4Defaults, IDE2Defaults, IDE6Defaults, IBE2Defaults
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

        public void ApplyDM3Defaults()
        {
            capacity = type switch
            {
                ResourceContainerType.Fuel => 1500,
                ResourceContainerType.Oil => 100,
                ResourceContainerType.Sand => 250,
                _ => 100
            };
            defaultValue = capacity;
        }

        public void ApplyDH4Defaults()
        {
            capacity = type switch
            {
                ResourceContainerType.Fuel => 6000,
                ResourceContainerType.Oil => 300,
                ResourceContainerType.Sand => 400,
                _ => 100,
            };
            defaultValue = capacity;
        }

        public void ApplyDE6Defaults()
        {
            capacity = type switch
            {
                ResourceContainerType.Fuel => 4000,
                ResourceContainerType.Oil => 500,
                ResourceContainerType.Sand => 2000,
                _ => 100,
            };
            defaultValue = capacity;
        }

        public void ApplyDE2Defaults()
        {
            capacity = type switch
            {
                ResourceContainerType.Fuel => 600,
                ResourceContainerType.Oil => 100,
                ResourceContainerType.Sand => 400,
                _ => 100,
            };
            defaultValue = capacity;
        }

        public void ApplyBE2Defaults()
        {
            capacity = type switch
            {
                ResourceContainerType.Sand => 400,
                _ => 100,
            };
            defaultValue = capacity;
        }
    }
}
