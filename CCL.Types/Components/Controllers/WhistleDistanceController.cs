using UnityEngine;

namespace CCL.Types.Components.Controllers
{
    [AddComponentMenu("CCL/Components/Controllers/Whistle Distance Controller")]
    public class WhistleDistanceController : MonoBehaviour
    {
        public GameObject DummyControl = null!;
        public Transform RelativeTo = null!;
        public float DistanceTolerance = 0.03f;
        public float MaxStrengthDistance = 0.35f;
    }
}
