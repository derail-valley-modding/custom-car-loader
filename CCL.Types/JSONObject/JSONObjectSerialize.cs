#nullable disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL.Types.Json
{
    partial class JSONObject
    {
        public static string ToJson(object obj)
        {
            return CreateFromObject(obj).ToString();
        }

        public static JSONObject CreateFromObject(object obj)
        {
            if (obj is ICollection arrayObj)
            {
                JSONObject result = NewArray;

                foreach (object item in arrayObj)
                {
                    result.Add(CreateFromObject(item));
                }
                return result;
            }

            if (obj is Enum e) return CreateStringObject(e.ToString());
            if (obj is bool b) return Create(b);
            if (obj is float f) return Create(f);
            if (obj is int i) return Create(i);
            if (obj is long l) return Create(l);
            if (obj is string s) return CreateStringObject(s);
            
            JSONObject json = NewObject;

            foreach (var kvp in GetObjectProperties(obj))
            {
                json.Keys.Add(kvp.Key);
                json.Children.Add(kvp.Value);
            }
            return json;
        }

        private static IEnumerable<KeyValuePair<string, JSONObject>> GetObjectProperties(object obj)
        {
            var members = GetSerializableMembers(obj.GetType());

            foreach (var member in members)
            {
                object value = member.GetValue(obj);

                yield return new KeyValuePair<string, JSONObject>(member.Name, CreateFromObject(value));
            }
        }

        private static IEnumerable<ValueMemberWrapper> GetSerializableMembers(Type objType)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var fields = objType.GetFields(flags).Where(CanSerializeMember).Select(f => new FieldMemberWrapper(f));
            var props = objType.GetProperties(flags).Where(CanSerializeMember).Select(p => new PropertyMemberWrapper(p));
                
            return fields.Cast<ValueMemberWrapper>().Concat(props).OrderBy(m => m.Name);
        }

        private static bool CanSerializeMember(MemberInfo member)
        {
            if (member.GetCustomAttribute<NonSerializedAttribute>() != null) return false;
            if (member.GetCustomAttribute<JsonIgnoreAttribute>() != null) return false;
            if (member.GetCustomAttribute<SerializeField>() != null) return true;
            if (member is FieldInfo field)
            {
                return field.IsPublic && !field.IsInitOnly;
            }
            if (member is PropertyInfo property)
            {
                return property.GetAccessors(false).Count() == 2;
            }
            return false;
        }

        public T ToObject<T>()
            where T : class
        {
            return ToObject(typeof(T)) as T;
        }

        private object ToObject(Type type, string name = null)
        {
            if (typeof(IDictionary<,>).IsAssignableFrom(type))
            {
                if (NodeType != JsonNodeType.OBJECT)
                {
                    throw new JsonSerializationException(name, $"Can't convert node {NodeType} to dictionary");
                }

                var generics = type.GetInterface(typeof(IDictionary<,>).Name).GetGenericArguments();
                Type tKey = generics[0], tValue = generics[1];
                IDictionary dictObj = (IDictionary)Activator.CreateInstance(type);

                if (tKey != typeof(string)) throw new JsonSerializationException(name, $"Can only convert dictionary with string keys");

                for (int i = 0; i < Count; i++)
                {
                    object value = Children[i].ToObject(tValue, $"{name} entry {i}");
                    dictObj.Add(Keys[i], value);
                }

                return dictObj;
            }

            if (typeof(IList).IsAssignableFrom(type) && (NodeType == JsonNodeType.ARRAY))
            {
                if (typeof(Array).IsAssignableFrom(type))
                {
                    var elementType = type.GetElementType();
                    var arrVal = Array.CreateInstance(elementType, Count);

                    for (int i = 0; i < Count; i++)
                    {
                        object realElement = Children[i].ToObject(elementType);
                        arrVal.SetValue(realElement, i);
                    }

                    return arrVal;
                }
                else if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)))
                {
                    var listVal = (IList)Activator.CreateInstance(type);
                    var elementType = type.GetGenericArguments()[0];

                    foreach (var child in Children)
                    {
                        object realElement = child.ToObject(elementType);
                        listVal.Add(realElement);
                    }

                    return listVal;
                }
                else throw new JsonSerializationException(name);
            }

            if (type.IsEnum)
            {
                if (NodeType != JsonNodeType.STRING) throw new JsonSerializationException(name);
                return Enum.Parse(type, StringVal, true);
            }
            if (type == typeof(bool))
            {
                if (NodeType != JsonNodeType.BOOL) throw new JsonSerializationException(name);
                return BoolVal;
            }
            else if (type == typeof(float))
            {
                if (NodeType != JsonNodeType.NUMBER) throw new JsonSerializationException(name);
                return FloatVal;
            }
            else if (type == typeof(int))
            {
                if (NodeType != JsonNodeType.NUMBER) throw new JsonSerializationException(name);
                return (int)IntVal;
            }
            else if (type == typeof(long))
            {
                if (NodeType != JsonNodeType.NUMBER) throw new JsonSerializationException(name);
                return IntVal;
            }
            else if (type == typeof(string))
            {
                if (NodeType != JsonNodeType.STRING) throw new JsonSerializationException(name);
                return StringVal;
            }

            // Generic object
            object result = Activator.CreateInstance(type);

            foreach (var member in GetSerializableMembers(type))
            {
                if (GetField(member.Name) is JSONObject memberJson)
                {
                    string subName = name != null ? $"{name}.{member.Name}" : member.Name;
                    object memberValue = memberJson.ToObject(member.Type, subName);
                    member.SetValue(result, memberValue);
                }
            }

            return result;
        }

        private abstract class ValueMemberWrapper
        {
            public abstract string Name { get; }
            public abstract Type Type { get; }

            public abstract object GetValue(object target);
            public abstract void SetValue(object target, object value);
        }

        private class FieldMemberWrapper : ValueMemberWrapper
        {
            private readonly FieldInfo _field;

            public override string Name => _field.Name;
            public override Type Type => _field.FieldType;
            public override object GetValue(object target) => _field.GetValue(target);
            public override void SetValue(object target, object value) => _field.SetValue(target, value);

            public FieldMemberWrapper(FieldInfo field)
            {
                _field = field;
            }
        }

        private class PropertyMemberWrapper : ValueMemberWrapper
        {
            private readonly PropertyInfo _property;

            public override string Name => _property.Name;
            public override Type Type => _property.PropertyType;
            public override object GetValue(object target) => _property.GetValue(target);
            public override void SetValue(object target, object value) => _property.SetValue(target, value);

            public PropertyMemberWrapper(PropertyInfo property)
            {
                _property = property;
            }
        }
    }

    public class JsonSerializationException : Exception
    {
        public readonly string MemberName;

        public JsonSerializationException(string memberName) :
            base($"Error serializing JSON member {memberName}")
        {
            MemberName = memberName;
        }

        public JsonSerializationException(string memberName, string message) :
            base($"Error serializing JSON member {memberName}: {message}")
        {
            MemberName = memberName;
        }

        public JsonSerializationException(string memberName, Exception inner) :
            base($"Error serializing JSON member {memberName}", inner)
        {
            MemberName = memberName;
        }

        public JsonSerializationException(string memberName, string message, Exception inner) :
            base($"Error serializing JSON member {memberName}: {message}", inner)
        {
            MemberName = memberName;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class JsonIgnoreAttribute : Attribute
    {

    }
}
