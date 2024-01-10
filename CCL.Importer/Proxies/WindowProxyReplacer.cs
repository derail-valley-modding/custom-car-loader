using AutoMapper;
using CCL.Types.Proxies;
using DV.Rain;
using System.ComponentModel.Composition;
using System.Linq;
using UnityEngine;

namespace CCL.Importer.Proxies
{
    [Export(typeof(IProxyReplacer))]
    public class WindowReplacer : Profile
    {
        public WindowReplacer()
        {
            CreateMap<WindowProxy, Window>()
                .ForMember(d => d.duplicates, o => o.MapFrom(s => s.duplicates.Select(w => Mapper.GetFromCache(w) ?? w)))
                .ForMember(nameof(Window.duplicates), o => o.ConvertUsing(new CacheConverter(o.DestinationMember), nameof(WindowProxy.duplicates)));
        }
    }
}
