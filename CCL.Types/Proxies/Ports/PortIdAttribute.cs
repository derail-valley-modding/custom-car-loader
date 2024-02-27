using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class PortIdAttribute : PropertyAttribute
    {
        public DVPortType[]? typeFilters;

        public DVPortValueType[]? valueTypeFilters;

        public bool local;

        public PortIdAttribute(DVPortType[]? typeFilters = null, DVPortValueType[]? valueTypeFilters = null, bool local = false)
        {
            this.typeFilters = typeFilters;
            this.valueTypeFilters = valueTypeFilters;
            this.local = local;
        }

        public PortIdAttribute(DVPortType typeFilter, DVPortValueType valueTypeFilter, bool local = false)
        {
            typeFilters = new DVPortType[1] { typeFilter };
            valueTypeFilters = new DVPortValueType[1] { valueTypeFilter };
            this.local = local;
        }

        public PortIdAttribute(DVPortType typeFilter, bool local = false)
        {
            typeFilters = new DVPortType[1] { typeFilter };
            valueTypeFilters = null;
            this.local = local;
        }

        public PortIdAttribute(DVPortValueType valueTypeFilter, bool local = false)
        {
            typeFilters = null;
            valueTypeFilters = new DVPortValueType[1] { valueTypeFilter };
            this.local = local;
        }
    }

    public enum DVPortValueType
    {
        GENERIC = 0,
        CONTROL = 50,
        STATE = 51,
        DAMAGE = 52,
        POWER = 100,
        TORQUE = 101,
        FORCE = 102,
        TEMPERATURE = 103,
        RPM = 104,
        AMPS = 105,
        VOLTS = 106,
        HEAT_RATE = 107,
        PRESSURE = 108,
        MASS_RATE = 109,
        OHMS = 110,
        FUEL = 200,
        OIL = 201,
        SAND = 202,
        WATER = 203,
        COAL = 204,
        ELECTRIC_CHARGE = 205
    }

    public enum DVPortType
    {
        IN,
        OUT,
        EXTERNAL_IN,
        READONLY_OUT,
        FORWARD_IN,
        FORWARD_OUT
    }
}
