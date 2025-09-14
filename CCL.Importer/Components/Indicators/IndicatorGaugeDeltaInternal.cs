using UnityEngine;

namespace CCL.Importer.Components.Indicators
{
    internal class IndicatorGaugeDeltaInternal : IndicatorGauge
    {
        public float updateThreshold = 0.001f;
        public float maxDelta = 20.0f;

        private bool needInitialValue = true;
        private float targetValue;
        private float previousValue;

        protected override void OnValueSet()
        {
            targetValue = value;

            if (needInitialValue)
            {
                previousValue = value;
                needInitialValue = false;
                SetNeedleRotation(value);
            }
            else
            {
                // Or else it snaps to the target right away.
                value = previousValue;
            }
        }

        private void Update()
        {
            var dif = targetValue - value;
            var abs = Mathf.Abs(dif);

            if (!assumeIsPaused && abs > updateThreshold)
            {
                previousValue = value;
                var delta = maxDelta * Time.deltaTime;
                value = abs <= delta ? targetValue : (value + Mathf.Sign(dif) * delta);
                SetNeedleRotation(value);
                FireValueChanged();
            }
        }
    }
}
