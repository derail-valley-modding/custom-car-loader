using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCL.Types.Proxies.Resources
{
    public enum ResourceContainerType
    {
        Coal,
        Fuel,
        Oil,
        Sand,
        Water
    }

    public static class ResourceContainerTypeExtensions
    {
        public static DVPortValueType PortValueType(this ResourceContainerType containerType)
        {
            return containerType switch
            {
                ResourceContainerType.Coal => DVPortValueType.COAL,
                ResourceContainerType.Fuel => DVPortValueType.FUEL,
                ResourceContainerType.Oil => DVPortValueType.OIL,
                ResourceContainerType.Sand => DVPortValueType.SAND,
                ResourceContainerType.Water => DVPortValueType.WATER,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
