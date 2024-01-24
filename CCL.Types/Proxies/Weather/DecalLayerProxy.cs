using System;
using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    // There's actually no need to show these in CCL as they use
    // the same values everywhere, so the defaults are set to
    // the correct values already.
    //[Serializable]
    public class DecalLayerProxy
    {
        private DecalLayerChannelProxy _channel2;
        private DecalLayerChannelProxy _channel3;
        private DecalLayerChannelProxy _channel4;
        private DecalLayerChannelProxy _channel1;
        private Texture2D _layerMask;
        private Vector2 _layerMaskOffset;
        private Vector2 _layerMaskScale;
        private bool _editorSectionFoldout;

        public DecalLayerProxy()
        {
            _layerMaskScale = new Vector2(1f, 1f);
            _layerMaskOffset = new Vector2(0f, 0f);

            _channel1 = new DecalLayerChannelProxy(DecalLayerChannelProxy.DecalChannelMode.SimpleRangeRemap);

            _channel2 = new DecalLayerChannelProxy();
            _channel3 = new DecalLayerChannelProxy();
            _channel4 = new DecalLayerChannelProxy();

            _layerMask = null!;
            _editorSectionFoldout = false;
        }
    }
}
