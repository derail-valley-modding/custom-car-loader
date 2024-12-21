using AutoMapper;
using CCL.Types.Proxies.Customization;
using DV.CabControls.Spec;
using DV.Customization.Paint;
using System.Collections.Generic;
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
            paint.sets = proxy.Sets.Select(x => ToSet(x)).ToArray();
        }

        private static TrainCarPaint.MaterialSet ToSet(MaterialSet set)
        {
            Object.Destroy(set);

            if (set is not DefaultBogieMaterialSet bogieSet)
            {
                return new TrainCarPaint.MaterialSet
                {
                    originalMaterial = set.OriginalMaterial,
                    renderers = set.Replacements.Select(r =>
                        new TrainCarPaint.RendererMaterialReplacement(r.Renderer, r.MaterialIndex)).ToArray()
                };
            }

            var cars = set.GetComponentsInParent<TrainCar>(true);

            if (cars.Length == 0)
            {
                CCLPlugin.Error("Could not find car for DefaultBogieMaterialSet.");
                return new TrainCarPaint.MaterialSet();
            }

            var bogies = cars[0].GetComponentsInChildren<Bogie>();

            if (bogies.Count() == 0)
            {
                CCLPlugin.Error("Could not find bogies for DefaultBogieMaterialSet.");
                return new TrainCarPaint.MaterialSet();
            }

            List<Renderer> renderers = new();
            Material? mat = null;

            foreach (var bogie in bogies)
            {
                foreach (var renderer in bogie.GetComponentsInChildren<Renderer>())
                {
                    mat ??= renderer.sharedMaterial;

                    if (renderer.sharedMaterial == mat)
                    {
                        renderers.Add(renderer);
                    }
                }
            }

            return new TrainCarPaint.MaterialSet
            {
                originalMaterial = mat,
                renderers = renderers.Select(r => new TrainCarPaint.RendererMaterialReplacement(r, 0)).ToArray()
            };
        }
    }
}
