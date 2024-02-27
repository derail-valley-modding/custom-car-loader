using CCL.Types.Proxies.Ports;
using System;

namespace CCL.Types.Proxies.Resources
{
    public enum ResourceContainerType
    {
        Coal,
        Fuel,
        Oil,
        Sand,
        Water,
        ElectricCharge
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
                ResourceContainerType.ElectricCharge => DVPortValueType.ELECTRIC_CHARGE,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
