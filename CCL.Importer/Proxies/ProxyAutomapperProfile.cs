using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Importer.Proxies
{
    public class ProxyAutomapperProfile : Profile
    {
        public ProxyAutomapperProfile() {
            foreach(var attribute in Attributes())
            {
                var map = CreateMap(attribute.SourceType, attribute.DestinationType);
                foreach(var cacheMapField in attribute.FieldsFromCache)
                {
                    map.ForMember(cacheMapField, cfg => {
                        cfg.ConvertUsing(new CacheConverter(cfg.DestinationMember), cacheMapField);
                    });
                }
            }
        }
        private IEnumerable<ProxyMapAttribute> Attributes()
        {
            return from t in Assembly.GetExecutingAssembly().GetTypes()
                   from a in t.GetCustomAttributes<ProxyMapAttribute>()
                   select a;
        }
    }
    public class CacheConverter : IValueConverter<object, object>
    {
        private readonly Type _destType;

        public CacheConverter(MemberInfo destMember)
        {
            if (destMember is FieldInfo field)
            {
                _destType = field.FieldType;
            }
            else if (destMember is PropertyInfo property)
            {
                _destType = property.PropertyType;
            }
            else throw new ArgumentException("Value converter used for non value member");
        }

        public object Convert(object sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null!;

            // check if T[]
            if (sourceMember is Array sourceArr)
            {
                Type targetElemType = _destType.GetElementType();
                Array destArray = Array.CreateInstance(targetElemType, sourceArr.Length);
                ConvertArrayValues(sourceArr, destArray);
                return destArray;
            }

            // check if List<T>
            Type sourceType = sourceMember.GetType();
            if (sourceType.IsConstructedGenericType && (typeof(List<>) == sourceType.GetGenericTypeDefinition()))
            {
                var destListType = typeof(List<>).MakeGenericType(_destType.GenericTypeArguments);
                var destList = (IList)Activator.CreateInstance(destListType);
                ConvertArrayValues((IList)sourceMember, destList);
                return destList;
            }

            // otherwise, single script
            if (!typeof(MonoBehaviour).IsInstanceOfType(sourceMember))
            {
                CCLPlugin.Warning("Attempted to map a non-monobehaviour");
                return sourceMember;
            }
            var result = Mapper.GetFromCache((MonoBehaviour)sourceMember);
            return result;
        }

        private static void ConvertArrayValues(IList source, IList dest)
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (!typeof(MonoBehaviour).IsInstanceOfType(source[i]))
                {
                    CCLPlugin.Warning("Attempted to map a non-monobehaviour");
                    dest[i] = null;
                }
                else
                {
                    dest[i] = Mapper.GetFromCache((MonoBehaviour)source[i]);
                }
            }
        }
    }
}
