using UnityEngine;

namespace CCL.Types.Catalog.Diagram
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class DiagramComponent : MonoBehaviour
    {
        public const float WIDTH = 350 / 2;
        public const float HEIGHT = 150 / 2;

        public static readonly Vector3[] FocusPoints = new[]
        {
            new Vector3(-WIDTH, -HEIGHT, 0),
            new Vector3(-WIDTH, HEIGHT, 0),
            new Vector3(WIDTH, HEIGHT, 0),
            new Vector3(WIDTH, -HEIGHT, 0)
        };

        public RectTransform RectTransform => GetComponent<RectTransform>();
    }
}
