using UnityEngine;

namespace CCL.Types.Components.Controls
{
    [AddComponentMenu("CCL/Components/Controls/Control Constant Feeder")]
    public class ControlConstantFeeder : MonoBehaviour
    {
        public GameObject ControlObject = null!;
        public float Multiplier = 1.0f;
        public bool Constant = false;
    }
}
