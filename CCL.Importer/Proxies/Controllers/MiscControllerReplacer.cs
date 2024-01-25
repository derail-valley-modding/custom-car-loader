using AutoMapper;
using CCL.Types.Proxies.Controllers;
using DV.Damage;

namespace CCL.Importer.Proxies.Controllers
{
    internal class MiscControllerReplacer : Profile
    {
        public MiscControllerReplacer()
        {
            CreateMap<DamageControllerProxy, DamageController>().AutoCacheAndMap();
        }
    }
}
