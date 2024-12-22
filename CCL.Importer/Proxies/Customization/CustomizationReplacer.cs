using AutoMapper;
using CCL.Types.Proxies.Customization;
using DV.Customization;
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

            CreateMap<TrainCarCustomizationProxy, TrainCarCustomization>().AutoCacheAndMap()
                .ForMember(d => d.standardizedPorts, o => o.MapFrom(s => s.Ports.Select(x =>
                    new TrainCarCustomization.STDPortDefinition((STDSimPort)x.port, x.name, x.readOnly))));

            CreateMap<CustomizationPlacementMeshesProxy, CustomizationPlacementMeshes>().AutoCacheAndMap();
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

            return set switch
            {
                DefaultBogieMaterialSet bogieSet => FromBogieSet(bogieSet),
                _ => new TrainCarPaint.MaterialSet
                {
                    originalMaterial = set.OriginalMaterial,
                    renderers = set.Replacements.Select(r =>
                        new TrainCarPaint.RendererMaterialReplacement(r.Renderer, r.MaterialIndex)).ToArray()
                },
            };
        }

        private static TrainCarPaint.MaterialSet FromBogieSet(DefaultBogieMaterialSet set)
        {
            // Need to find the bogies, so grab the car somewhere up the hierarchy
            // then find the child bogies.
            var car = set.GetComponentInParentIncludingInactive<TrainCar>();

            if (car == null)
            {
                CCLPlugin.Error("Could not find car for DefaultBogieMaterialSet.");
                return new TrainCarPaint.MaterialSet();
            }

            var bogies = car.GetComponentsInChildren<Bogie>();

            if (bogies.Length == 0)
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
                    // Assign the first material as the main one.
                    mat ??= renderer.sharedMaterial;

                    // And all matching ones afterwards.
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
