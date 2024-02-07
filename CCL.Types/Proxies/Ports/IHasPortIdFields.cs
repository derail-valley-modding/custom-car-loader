using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class PortIdField : IdFieldBase
    {
        public readonly DVPortType[]? TypeFilters;
        public readonly DVPortValueType[]? ValueFilters;

        public bool CanConnect(DVPortType type)
        {
            return (TypeFilters == null) || TypeFilters.Contains(type);
        }

        public bool CanConnect(DVPortValueType valueType)
        {
            return (ValueFilters == null) || ValueFilters.Contains(valueType);
        }

        // multi id, multi type, multi value
        public PortIdField(MonoBehaviour container, string fieldName, string[]? portIds, DVPortType[]? typeFilters = null, DVPortValueType[]? valueFilters = null)
            : base(container, fieldName, portIds)
        {
            TypeFilters = typeFilters;
            ValueFilters = valueFilters;
        }

        // multi id, single type
        public PortIdField(MonoBehaviour container, string fieldName, string[]? portIds, DVPortType typeFilter)
            : this(container, fieldName, portIds, new[] { typeFilter })
        { }

        // multi id, single value
        public PortIdField(MonoBehaviour container, string fieldName, string[]? portIds, DVPortValueType valueFilter)
            : this(container, fieldName, portIds, valueFilters: new[] { valueFilter })
        { }

        // multi id, single type, single value
        public PortIdField(MonoBehaviour container, string fieldName, string[]? portIds, DVPortType typeFilter, DVPortValueType valueFilter)
            : this(container, fieldName, portIds, new[] { typeFilter }, new[] { valueFilter })
        { }

        // single id, multi type, multi value
        public PortIdField(MonoBehaviour container, string fieldName, string portId, DVPortType[]? typeFilters = null, DVPortValueType[]? valueFilters = null)
            : this(container, fieldName, new[] { portId }, typeFilters, valueFilters)
        { }

        // single id, single type
        public PortIdField(MonoBehaviour container, string fieldName, string portId, DVPortType typeFilter)
            : this(container, fieldName, portId, typeFilters: new[] { typeFilter })
        { }

        // single id, single value
        public PortIdField(MonoBehaviour container, string fieldName, string portId, DVPortValueType valueFilter)
            : this(container, fieldName, portId, valueFilters: new[] { valueFilter })
        { }

        // single id, single type, single value
        public PortIdField(MonoBehaviour container, string fieldName, string portId, DVPortType typeFilter, DVPortValueType valueFilter)
            : this(container, fieldName, portId, new[] { typeFilter }, new[] { valueFilter })
        { }
    }

    public interface IHasPortIdFields
    {
        IEnumerable<PortIdField> ExposedPortIdFields { get; }
    }
}
