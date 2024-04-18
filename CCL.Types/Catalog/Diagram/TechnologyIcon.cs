using UnityEngine;

namespace CCL.Types.Catalog.Diagram
{
    public class TechnologyIcon : DiagramComponent
    {
        public const float SIZE = 45 / 2;

        private static float[] s_gridLinesX = new float[]
        {
            -137.5f,
            -130,
            -110,
            -82.5f,
            -55,
            -27.5f,
            0,
            27.5f,
            55,
            82.5f,
            110,
            130,
            137.5f,
        };

        private static float[] s_gridLinesY = new float[]
        {
            -40,
            -30,
            -15,
            10,
            15,
            25,
            40,
        };

        public TechIcon Icon;
        public bool Flip = false;

        public override void AlignToGrid()
        {
            transform.localPosition = new Vector3(
                s_gridLinesX.ClosestTo(transform.localPosition.x),
                s_gridLinesY.ClosestTo(transform.localPosition.y),
                transform.localPosition.z);
        }
    }
}
