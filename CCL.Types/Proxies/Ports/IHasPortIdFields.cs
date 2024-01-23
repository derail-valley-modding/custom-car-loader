using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class PortIdField
    {
        public readonly MonoBehaviour Container;
        public readonly string FieldName;
        public readonly string AssignedPort;

        public readonly DVPortType[]? TypeFilters;
        public readonly DVPortValueType[]? ValueFilters;

        public bool IsAssigned => !string.IsNullOrWhiteSpace(AssignedPort);
        public string FullName => $"{Container.name}.{FieldName}";

        public PortIdField(MonoBehaviour container, string fieldName, string portId, DVPortType[]? typeFilters = null, DVPortValueType[]? valueTypeFilters = null)
        {
            Container = container;
            FieldName = fieldName;
            AssignedPort = portId;

            TypeFilters = typeFilters;
            ValueFilters = valueTypeFilters;
        }

        public PortIdField(MonoBehaviour container, string fieldName, string portId, DVPortType typeFilter)
            : this(container, fieldName, portId, typeFilters: new[] { typeFilter })
        { }

        public PortIdField(MonoBehaviour container, string fieldName, string portId, DVPortValueType valueFilter)
            : this(container, fieldName, portId, valueTypeFilters: new[] { valueFilter })
        { }

        private static readonly BindingFlags AllInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public void Assign(string fullPortId)
        {
            if (Container.GetType().GetField(FieldName, AllInstance) is FieldInfo field)
            {
                field.SetValue(Container, fullPortId);
            }
        }

        public void Destroy()
        {
            if (Container.GetType().GetField(FieldName, AllInstance) is FieldInfo field)
            {
                field.SetValue(Container, null);
            }
        }
    }

    public interface IHasPortIdFields
    {
        IEnumerable<PortIdField> ExposedPortIdFields { get; }
    }
}
