using System.Linq;
using UnityEngine;

namespace CCL.Types.Catalog.Diagram
{
    [AddComponentMenu("CCL/Catalog/Bogie Layout")]
    public class BogieLayout : DiagramComponent
    {
        public const float BOGIE_HEIGHT = -75;
        public const float RADIUS = 20;
        public const float MIDDLE_SPACE = 5;

        private static float[] s_gridLines = new float[]
        {
            -125,
            -114,
            -100,
            -42,
            0,
            42,
            100,
            114,
            125,
        };

        [Tooltip("True if this bogie rotates, false if it is mounted directly on the frame")]
        public bool Pivots = true;
        [Tooltip("The number of wheels, and whether they are powered or not")]
        public bool[] Wheels = new[] { true, true };

        public void OnValidate()
        {
            transform.localPosition = new Vector3(transform.localPosition.x, BOGIE_HEIGHT, transform.localPosition.z);
        }

        public static void TryToAlignBogies(Transform commonParent)
        {
            var bogies = commonParent.GetComponentsInChildren<BogieLayout>().OrderBy(x => x.transform.localPosition.x).ToArray();
            int count = bogies.Length;
            Transform t;

            if (count == 1)
            {
                t = bogies[0].transform;
                t.localPosition = new Vector3(0, BOGIE_HEIGHT, t.localPosition.z);
            }
            else
            {
                float step = 1.0f / (count - 1);
                float range = count < 3 ? 100 : 125;

                for (int i = 0; i < count; i++)
                {
                    t = bogies[i].transform;
                    t.localPosition = new Vector3(Mathf.Lerp(-range, range, i * step), BOGIE_HEIGHT, t.localPosition.z);
                }
            }
        }

        public override void AlignToGrid()
        {
            transform.localPosition = new Vector3(
                s_gridLines.ClosestTo(transform.localPosition.x),
                BOGIE_HEIGHT, transform.localPosition.z);
        }
    }
}
