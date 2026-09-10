using UnityEngine;

namespace CCL.Types.Catalog.Diagram
{
    [AddComponentMenu("CCL/Catalog/Vehicle Body")]
    public class VehicleBody : DiagramComponent
    {
        private const float DM1U_TALL_WIDTH = 133.4f;

        private static readonly float[] s_linesH = new float[]
        {
            -WIDTH + DM1U_TALL_WIDTH / 2,
            0,
            WIDTH - DM1U_TALL_WIDTH / 2,
        };
        private static readonly float[] s_linesV = new float[]
        {
            -HEIGHT + 90 / 2,
            0,
            HEIGHT - 90 / 2,
        };
        private static readonly float[] s_widths = new float[]
        {
            DM1U_TALL_WIDTH,
            350
        };
        private static readonly float[] s_heights = new float[]
        {
            90,
            150
        };

        //public void OnValidate()
        //{
        //    var rect = (RectTransform)transform;
        //    var delta = rect.sizeDelta / 2;

        //    rect.position = new Vector3(
        //        Mathf.Clamp(rect.position.x, Mathf.Max(-WIDTH, rect.position.x - delta.x), Mathf.Max(WIDTH, rect.position.x + delta.x)),
        //        Mathf.Clamp(rect.position.y, Mathf.Max(-HEIGHT, rect.position.y - delta.y), Mathf.Max(HEIGHT, rect.position.y + delta.y)),
        //        0);
        //}

        public override void AlignToGrid()
        {
            var rect = (RectTransform)transform;
            rect.position = new Vector3(s_linesH.ClosestTo(transform.position.x), s_linesV.ClosestTo(transform.position.y), 0);
            rect.sizeDelta = new Vector2(s_widths.ClosestTo(rect.sizeDelta.x), s_heights.ClosestTo(rect.sizeDelta.y));
        }
    }
}
