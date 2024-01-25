using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class PortIdField
    {
        public readonly MonoBehaviour Container;
        public readonly string FieldName;
        public readonly string[]? AssignedPorts;

        public readonly DVPortType[]? TypeFilters;
        public readonly DVPortValueType[]? ValueFilters;

        public bool IsAssigned => (AssignedPorts != null) && (AssignedPorts.Length > 0);
        public bool IsPortAssigned(string fullPortId)
        {
            return (AssignedPorts != null) && AssignedPorts.Contains(fullPortId);
        }

        public string FullName => $"{Container.name}.{FieldName}";

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
        {
            Container = container;
            FieldName = fieldName;
            AssignedPorts = portIds;

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

        private static readonly BindingFlags AllInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public void Assign(string fullPortId)
        {
            if (Container.GetType().GetField(FieldName, AllInstance) is FieldInfo field)
            {
                if (typeof(string[]) == field.FieldType)
                {
                    if (!(field.GetValue(Container) is string[] array) || (array.Length == 0))
                    {
                        // array is null or empty
                        array = new string[1] { fullPortId };
                        field.SetValue(Container, array);
                    }
                    else
                    {
                        // existing array
                        if (!array.Contains(fullPortId))
                        {
                            array = array.Append(fullPortId).ToArray();
                            field.SetValue(Container, array);
                        }
                    }
                }
                else if (typeof(List<string>).IsAssignableFrom(field.FieldType))
                {
                    if (!(field.GetValue(Container) is List<string> list))
                    {
                        list = new List<string>();
                        field.SetValue(Container, list);
                    }

                    if (!list.Contains(fullPortId))
                    {
                        list.Add(fullPortId);
                    }
                }
                else
                {
                    field.SetValue(Container, fullPortId);
                }
            }
        }

        public void UnAssign(string fullPortId)
        {
            if (Container.GetType().GetField(FieldName, AllInstance) is FieldInfo field)
            {
                if (typeof(string[]) == field.FieldType)
                {
                    if ((field.GetValue(Container) is string[] array) && (array.Length > 0))
                    {
                        // existing array
                        if (array.Contains(fullPortId))
                        {
                            array = array.Where(id => id != fullPortId).ToArray();
                            field.SetValue(Container, array);
                        }
                    }
                }
                else if (typeof(List<string>).IsAssignableFrom(field.FieldType))
                {
                    if (field.GetValue(Container) is List<string> list)
                    {
                        list.Remove(fullPortId);
                    }
                }
                else
                {
                    field.SetValue(Container, null);
                }
            }
        }
    }

    public interface IHasPortIdFields
    {
        IEnumerable<PortIdField> ExposedPortIdFields { get; }
    }
}
