using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public abstract class IndicatorProxy : MonoBehaviour
    {
        public float minValue;
        public float maxValue = 1f;

        // To show the enable/disable checkbox.
        public void Start() { }
    }
}
