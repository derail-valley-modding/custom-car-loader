using UnityEngine;

namespace CCL.Types.Components.Materials
{
    public class GenerateSightGlass : GeneratedMaterial<GenerateSightGlass>
    {
        public Vector2 BackgroundTextureTiling = new Vector2(1.0f, 6.0f);
        public Vector2 GlassTextureTiling = new Vector2(0.2f, 1.0f);

        public float PipeThickness = 0.5f;

        public Color LiquidTint = new Color(0.6673193f, 0.7704878f, 0.8679245f, 1f);
        public Color GlassTint = new Color(0.754717f, 0.6976369f, 0.494838f, 1f);

        public override bool AreSameSettings(GenerateSightGlass other)
        {
            return BackgroundTextureTiling == other.BackgroundTextureTiling &&
                GlassTextureTiling == other.GlassTextureTiling &&
                PipeThickness == other.PipeThickness &&
                LiquidTint == other.LiquidTint &&
                GlassTint == other.GlassTint;
        }
    }
}
