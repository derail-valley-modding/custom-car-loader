using AutoMapper;
using CCL.Types.Proxies;
using DV.Rain;
using System.ComponentModel.Composition;
using System.Linq;

namespace CCL.Importer.Proxies
{
    [Export(typeof(IProxyReplacer))]
    public class WindowReplacer : ProxyReplacer<WindowProxy, Window>
    {
        protected override void Customize(IMappingExpression<WindowProxy, Window> cfg)
        {
            cfg.ForMember(d => d.duplicates, o => o.MapFrom(s => s.duplicates.Select(w => Mapper.GetFromCache(w))));
        }
    }
}
