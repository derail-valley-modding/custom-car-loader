using AutoMapper;
using System;
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
                        cfg.ConvertUsing(typeof(CacheConverter<,>));
                    });
                }
            }
        }
        private IEnumerable<ProxyMapAttribute> Attributes()
        {
            return from t in Assembly.GetExecutingAssembly().GetTypes()
                   from a in t.GetCustomAttributes<ProxyMapAttribute>()
                   where t.Namespace.StartsWith("CCL.Importer.Proxies")
                   select a;
        }
    }
    public class CacheConverter<TSourceType, TDestinationType> : IValueConverter<TSourceType, TDestinationType>
        where TSourceType : MonoBehaviour
        where TDestinationType : MonoBehaviour
    {
        public TDestinationType Convert(TSourceType sourceMember, ResolutionContext context) => Mapper.GetFromCache(sourceMember) as TDestinationType;
    }
}
