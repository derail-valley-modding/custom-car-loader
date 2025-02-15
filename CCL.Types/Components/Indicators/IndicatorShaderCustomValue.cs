using CCL.Types.Proxies.Indicators;
using UnityEngine;

namespace CCL.Types.Components.Indicators
{
    public class IndicatorShaderCustomValue : IndicatorProxy
    {
        public MeshRenderer Renderer = null!;
        public string PropertyId = string.Empty;
    }
}
