using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public abstract class IdFieldBase
    {
        private const BindingFlags AllInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public readonly MonoBehaviour Container;
        public readonly string FieldName;
        public readonly bool IsMultiValue;
        public readonly string[] AssignedIds;
        public readonly bool Required;

        public string FullName => $"{Container.name}/{FieldName}";

        public bool IsAssigned => AssignedIds.Length > 0;
        public bool IsIdAssigned(string fullFuseId)
        {
            return AssignedIds.Contains(fullFuseId);
        }

        public IdFieldBase(MonoBehaviour container, string fieldName, string[] assignedIds, bool required)
        {
            Container = container;
            FieldName = fieldName;
            AssignedIds = assignedIds.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            Required = required;

            IsMultiValue = Container.GetType().GetField(FieldName, AllInstance).FieldType != typeof(string);
        }

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
                            field.SetValue(Container, array.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray());
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

                    list.RemoveAll(string.IsNullOrWhiteSpace);

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
                            array = array.Where(id => id != fullPortId && !string.IsNullOrWhiteSpace(id)).ToArray();
                            field.SetValue(Container, array);
                        }
                    }
                }
                else if (typeof(List<string>).IsAssignableFrom(field.FieldType))
                {
                    if (field.GetValue(Container) is List<string> list)
                    {
                        list.Remove(fullPortId);
                        list.RemoveAll(string.IsNullOrWhiteSpace);
                    }
                }
                else
                {
                    field.SetValue(Container, null);
                }
            }
        }
    }
}
