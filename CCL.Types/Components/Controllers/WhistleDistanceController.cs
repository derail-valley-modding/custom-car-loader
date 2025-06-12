using UnityEngine;

namespace CCL.Types.Components.Controllers
{
    public class WhistleDistanceController : MonoBehaviour
    {
        public GameObject DummyControl = null!;
        public Transform RelativeTo = null!;
        public float DistanceTolerance = 0.03f;
        public float MaxStrenghtDistance = 0.25f;
    }
}
