using UnityEngine;

namespace CCL.Types.Components.Controls
{
    [AddComponentMenu("CCL/Components/Controls/Control Ratchet Feeder")]
    public class ControlRatchetFeeder : MonoBehaviour
    {
        public GameObject ControlObject = null!;
        public float Multiplier = 1.0f;
        public bool Reverse = false;
    }
}
