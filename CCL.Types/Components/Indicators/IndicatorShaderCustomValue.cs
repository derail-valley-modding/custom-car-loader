using CCL.Types.Proxies.Indicators;
using UnityEngine;

namespace CCL.Types.Components.Indicators
{
    [AddComponentMenu("CCL/Components/Indicators/Indicator Shader Custom Value")]
    public class IndicatorShaderCustomValue : IndicatorProxy
    {
        public MeshRenderer Renderer = null!;
        public string PropertyId = string.Empty;
    }
}
