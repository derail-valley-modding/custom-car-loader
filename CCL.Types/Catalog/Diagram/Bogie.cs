using System.Linq;
using UnityEngine;

namespace CCL.Types.Catalog.Diagram
{
    public class Bogie : DiagramComponent
    {
        public const float BOGIE_HEIGHT = -75;
        public const float RADIUS = 20;

        [Tooltip("True if this bogie rotates, false if it is mounted directly on the frame")]
        public bool Pivots = true;
        [Tooltip("The number of wheels, and whether they are powered or not")]
        public bool[] Wheels = new[] { true, true };

        public void OnValidate()
        {
            var t = RectTransform;
            t.localPosition = new Vector3(t.localPosition.x, BOGIE_HEIGHT, t.localPosition.z);
        }

        public static void TryToAlignBogies(Transform commonParent)
        {
            var bogies = commonParent.GetComponentsInChildren<Bogie>().OrderBy(x => x.transform.localPosition.x).ToArray();
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
    }
}
