using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    public class DecalLayerChannelProxy
    {
        public enum DecalChannelMode
        {
            Disabled,
            Passthrough,
            SimpleRangeRemap,
            AdvancedRangeRemap
        }

        private float _inputRangeSoftness;
        private float _inputRangeThreshold;
        private DecalChannelMode _mode;
        private Vector2 _outputRange;

        public DecalLayerChannelProxy(DecalChannelMode mode)
        {
            _mode = mode;
            _inputRangeSoftness = 0.5f;
            _inputRangeThreshold = 1.0f;
            _outputRange = new Vector2(0.0f, 1.0f);
        }

        public DecalLayerChannelProxy() : this(DecalChannelMode.Disabled) { }
    }
}
