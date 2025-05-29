using CCL.Types;
using CCL.Types.Components.Materials;
using UnityEngine;

namespace CCL.Importer
{
    internal static class ProceduralMaterialGenerator
    {
        private static class ShaderProps
        {
            // Standard
            public static readonly int MainTex = Shader.PropertyToID("_MainTex");
            public static readonly int OcclusionStrength = Shader.PropertyToID("_OcclusionStrength");
            public static readonly int OcclusionMap = Shader.PropertyToID("_OcclusionMap");
            public static readonly int NormalMap = Shader.PropertyToID("_BumpMap");

            // Standard Detail
            public static readonly int DetailAlbedoMap = Shader.PropertyToID("_DetailAlbedoMap");
            public static readonly int DetailNormalMap = Shader.PropertyToID("_DetailNormalMap");
            public static readonly int DetailNormalScale = Shader.PropertyToID("_DetailNormalMapScale");

            // DV/SightGlass
            public static readonly int SightGlassThickness = Shader.PropertyToID("_PipeThickness");
            public static readonly int SightGlassGlass = Shader.PropertyToID("_GlassDetail");
            public static readonly int SightGlassWaterTint = Shader.PropertyToID("_WaterTint");
            public static readonly int SightGlassGlassTint = Shader.PropertyToID("_GlassTint");
        }

        public static void Generate(ProceduralMaterialDefinitions definitions)
        {
            foreach (var definition in definitions.Entries)
            {
                Generate(definition);
            }
        }

        private static void Generate(ProceduralMaterialDefinitions.GeneratorSet definition)
        {
            switch (definition.Type)
            {
                case ProceduralMaterialDefinitions.MaterialType.Exploded:
                    GenerateExploded(definition.Original);
                    return;
                case ProceduralMaterialDefinitions.MaterialType.PaintDetailsOld:
                    GeneratePaintDetailsOld(definition.Original);
                    return;
                case ProceduralMaterialDefinitions.MaterialType.PaintDetailsNew:
                    GeneratePaintDetailsNew(definition.Original);
                    return;
                case ProceduralMaterialDefinitions.MaterialType.PaintDetailsRusted:
                    GeneratePaintDetailsRusted(definition.Original);
                    return;
                case ProceduralMaterialDefinitions.MaterialType.Primer:
                    GeneratePrimer(definition.Original);
                    return;
                default:
                    return;
            }
        }

        public static void GenerateExploded(Material original)
        {
            var strength = original.GetFloat(ShaderProps.OcclusionStrength);
            var map = original.GetTexture(ShaderProps.OcclusionMap);

            original.CopyPropertiesFromMaterial(QuickAccess.Materials.ExplodedDE2Cab);
            original.SetFloat(ShaderProps.OcclusionStrength, strength);
            original.SetTexture(ShaderProps.OcclusionMap, map);
        }

        public static void GeneratePaintDetailsOld(Material original)
        {
            var mat = QuickAccess.Materials.BodyDE2;

            original.SetTexture(ShaderProps.DetailAlbedoMap, mat.GetTexture(ShaderProps.DetailAlbedoMap));
            original.SetTexture(ShaderProps.DetailNormalMap, mat.GetTexture(ShaderProps.DetailNormalMap));
            original.SetFloat(ShaderProps.DetailNormalScale, mat.GetFloat(ShaderProps.DetailNormalScale));
        }

        public static void GeneratePaintDetailsNew(Material original)
        {
            var mat = QuickAccess.Materials.BodyDE2New;

            original.SetTexture(ShaderProps.DetailAlbedoMap, mat.GetTexture(ShaderProps.DetailAlbedoMap));
            original.SetTexture(ShaderProps.DetailNormalMap, mat.GetTexture(ShaderProps.DetailNormalMap));
            original.SetFloat(ShaderProps.DetailNormalScale, mat.GetFloat(ShaderProps.DetailNormalScale));
        }

        public static void GeneratePaintDetailsRusted(Material original)
        {
            var mat = QuickAccess.Materials.BodyDE2;

            original.SetTexture(ShaderProps.DetailAlbedoMap, mat.GetTexture(ShaderProps.DetailAlbedoMap));
            original.SetTexture(ShaderProps.DetailNormalMap, mat.GetTexture(ShaderProps.DetailNormalMap));
            original.SetFloat(ShaderProps.DetailNormalScale, mat.GetFloat(ShaderProps.DetailNormalScale));
        }

        public static void GeneratePrimer(Material original)
        {
            var strength = original.GetFloat(ShaderProps.OcclusionStrength);
            var ao = original.GetTexture(ShaderProps.OcclusionMap);
            var normal = original.GetTexture(ShaderProps.NormalMap);

            original.CopyPropertiesFromMaterial(QuickAccess.Materials.PrimerDE2);
            original.SetFloat(ShaderProps.OcclusionStrength, strength);
            original.SetTexture(ShaderProps.OcclusionMap, ao);
            original.SetTexture(ShaderProps.NormalMap, normal);
        }

        public static Material GenerateMaterial(IGeneratedMaterial generator)
        {
            return generator switch
            {
                GenerateSightGlass sightglass => GenerateSightGlassMat(sightglass),
                _ => null!,
            };
        }

        public static Material GenerateSightGlassMat(GenerateSightGlass settings)
        {
            if (GenerateSightGlass.TryGetAlreadyGenerated(settings, out var mat)) return mat;
            
            mat = new Material(QuickAccess.Materials.SightGlassS060);

            mat.SetTextureScale(ShaderProps.MainTex, settings.BackgroundTextureTiling);
            mat.SetTextureScale(ShaderProps.SightGlassGlass, settings.GlassTextureTiling);
            mat.SetVector(ShaderProps.SightGlassWaterTint, settings.LiquidTint);
            mat.SetVector(ShaderProps.SightGlassGlassTint, settings.GlassTint);
            mat.SetFloat(ShaderProps.SightGlassThickness, settings.PipeThickness);

            settings.Cache(mat);

            return mat;
        }
    }
}
