using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public abstract class IndicatorProxy : MonoBehaviour
    {
        protected static readonly Color START_COLOR = new Color(0.65f, 0, 0);
        protected static readonly Color END_COLOR = new Color(0, 0, 0.65f);
        protected static readonly Color MID_COLOR = new Color(0, 0.65f, 0);

        public float minValue;
        public float maxValue = 1f;
    }
}
