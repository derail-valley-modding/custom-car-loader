using AutoMapper;
using CCL.Types.Proxies.VerletRope;
using UnityEngine;
using VerletRope;

using RopeParamsDV = VerletRope.RopeParams;

namespace CCL.Importer.Proxies.VerletRope
{
    internal class VerletRopeReplacer : Profile
    {
        public VerletRopeReplacer()
        {
            CreateMap<PointFollowerProxy, PointFollower>().AutoCacheAndMap()
                .ForMember(d => d.rope, o => o.MapFrom(s => Mapper.GetFromCache(s.rope)));
            CreateMap<RopeBehaviourProxy, RopeBehaviour>().AutoCacheAndMap()
                .ForMember(d => d.meshGenerator, o => o.MapFrom(s => Mapper.GetFromCache(s.meshGenerator)))
                .AfterMap(RopeBehaviourAfter);
            CreateMap<RopeMeshGeneratorProxy, RopeMeshGenerator>().AutoCacheAndMap();

            CreateMap<CCL.Types.Proxies.VerletRope.RopeParams, RopeParamsDV>();
            CreateMap<RopePin, Pin>();
        }

        private void RopeBehaviourAfter(RopeBehaviourProxy proxy, RopeBehaviour rope)
        {
            foreach (var item in proxy.pins)
            {
                Object.Destroy(item);
            }

            rope.ropeParams.floorLevel = proxy.ropeParams.floorLevel;
        }
    }
}
