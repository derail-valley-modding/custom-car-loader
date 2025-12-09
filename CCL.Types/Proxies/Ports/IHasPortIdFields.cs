using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class PortIdField : IdFieldBase
    {
        public readonly DVPortType[]? TypeFilters;
        public readonly DVPortValueType[]? ValueFilters;
        public readonly bool Required = true;

        public bool IsValid => !Required || IsAssigned;

        public bool CanConnect(DVPortType type)
        {
            return (TypeFilters == null) || TypeFilters.Contains(type);
        }

        public bool CanConnect(DVPortValueType valueType)
        {
            return (ValueFilters == null) || ValueFilters.Contains(valueType);
        }

        // multi id, multi type, multi value
        public PortIdField(MonoBehaviour container, string fieldName, string[] portIds,
            DVPortType[]? typeFilters = null, DVPortValueType[]? valueFilters = null, bool required = true)
            : base(container, fieldName, portIds)
        {
            TypeFilters = typeFilters;
            ValueFilters = valueFilters;
            Required = required;
        }

        // multi id, single type
        public PortIdField(MonoBehaviour container, string fieldName, string[] portIds,
            DVPortType typeFilter, bool required = true)
            : this(container, fieldName, portIds, new[] { typeFilter }, required: required)
        { }

        // multi id, single value
        public PortIdField(MonoBehaviour container, string fieldName, string[] portIds,
            DVPortValueType valueFilter, bool required = true)
            : this(container, fieldName, portIds, valueFilters: new[] { valueFilter }, required: required)
        { }

        // multi id, single type, single value
        public PortIdField(MonoBehaviour container, string fieldName, string[] portIds,
            DVPortType typeFilter, DVPortValueType valueFilter, bool required = true)
            : this(container, fieldName, portIds, new[] { typeFilter }, new[] { valueFilter }, required)
        { }

        // single id, multi type, multi value
        public PortIdField(MonoBehaviour container, string fieldName, string portId,
            DVPortType[]? typeFilters = null, DVPortValueType[]? valueFilters = null, bool required = true)
            : this(container, fieldName, new[] { portId }, typeFilters, valueFilters, required)
        { }

        // single id, single type
        public PortIdField(MonoBehaviour container, string fieldName, string portId,
            DVPortType typeFilter, bool required = true)
            : this(container, fieldName, portId, typeFilters: new[] { typeFilter }, required: required)
        { }

        // single id, single value
        public PortIdField(MonoBehaviour container, string fieldName, string portId,
            DVPortValueType valueFilter, bool required = true)
            : this(container, fieldName, portId, valueFilters: new[] { valueFilter }, required: required)
        { }

        // single id, single type, single value
        public PortIdField(MonoBehaviour container, string fieldName, string portId,
            DVPortType typeFilter, DVPortValueType valueFilter, bool required = true)
            : this(container, fieldName, portId, new[] { typeFilter }, new[] { valueFilter }, required)
        { }
    }

    public interface IHasPortIdFields
    {
        IEnumerable<PortIdField> ExposedPortIdFields { get; }
    }
}
