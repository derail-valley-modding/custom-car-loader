using System.Collections.Generic;
using CCL.Types.Proxies.Ports;

namespace CCL.Types.Proxies.Resources
{
    public class ResourceContainerProxy : SimComponentDefinitionProxy, IDM3Defaults, IDH4Defaults, IDE2Defaults, IDE6Defaults, IBE2Defaults, IS060Defaults, IS282Defaults
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
                    new PortDefinition(DVPortType.READONLY_OUT, valueType, "CAPACITY"),
                    new PortDefinition(DVPortType.EXTERNAL_IN, valueType, "REFILL_EXT_IN"),
                    new PortDefinition(DVPortType.EXTERNAL_IN, valueType, "CONSUME_EXT_IN"),
                    new PortDefinition(DVPortType.READONLY_OUT, valueType, "AMOUNT"),
                    new PortDefinition(DVPortType.READONLY_OUT, valueType, "NORMALIZED"),
                };
            }
        }

        #region Defaults

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
                ResourceContainerType.ElectricCharge => 360,
                _ => 100,
            };
            defaultValue = capacity;
        }

        public void ApplyS060Defaults()
        {
            capacity = type switch
            {
                ResourceContainerType.Coal => 10000,
                ResourceContainerType.Oil => 100,
                ResourceContainerType.Sand => 600,
                ResourceContainerType.Water => 30000,
                _ => 100,
            };
            defaultValue = capacity;
        }

        public void ApplyS282Defaults()
        {
            capacity = type switch
            {
                ResourceContainerType.Coal => 1200,
                ResourceContainerType.Oil => 100,
                ResourceContainerType.Sand => 240,
                ResourceContainerType.Water => 4500,
                _ => 100,
            };
            defaultValue = capacity;
        }

        #endregion
    }
}
