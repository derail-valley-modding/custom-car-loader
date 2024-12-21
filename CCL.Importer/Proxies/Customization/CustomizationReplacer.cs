using AutoMapper;
using CCL.Types.Proxies.Customization;
using DV.Customization.Paint;
using System.Linq;
using UnityEngine;

namespace CCL.Importer.Proxies.Customization
{
    internal class CustomizationReplacer : Profile
    {
        public CustomizationReplacer()
        {
            CreateMap<TrainCarPaintProxy, TrainCarPaint>().AutoCacheAndMap()
                .ForMember(d => d.currentTheme, o => o.Ignore())
                .ForMember(d => d.CurrentTheme, o => o.Ignore())
                .ForMember(d => d.sets, o => o.Ignore())
                .AfterMap(TrainCarPaintAfter);
        }

        private void TrainCarPaintAfter(TrainCarPaintProxy proxy, TrainCarPaint paint)
        {
            if (!PaintTheme.TryLoad(proxy.CurrentTheme, out var theme))
            {
                CCLPlugin.Error($"Could not find paint {proxy.CurrentTheme}");
                Object.Destroy(paint);
                return;
            }

            paint.currentTheme = theme;
            paint.sets = proxy.Sets.Select(x =>
                new TrainCarPaint.MaterialSet
                {
                    originalMaterial = x.OriginalMaterial,
                    renderers = x.Replacements.Select(r =>
                        new TrainCarPaint.RendererMaterialReplacement(r.Renderer, r.MaterialIndex)).ToArray()
                }).ToArray();

            foreach (var item in proxy.Sets)
            {
                Object.Destroy(item);
            }
        }
    }
}
