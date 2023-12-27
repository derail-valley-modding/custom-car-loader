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
                        cfg.ConvertUsing(typeof(CacheConverter), cacheMapField);
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
        public object Convert(object sourceMember, ResolutionContext context)
        {
            if (!typeof(MonoBehaviour).IsInstanceOfType(sourceMember))
            {
                CCLPlugin.Warning("Attempted to map a non-monobehaviour");
                return sourceMember;
            }
            var result = Mapper.GetFromCache(sourceMember as MonoBehaviour);
            return result;
        }
    }
}
