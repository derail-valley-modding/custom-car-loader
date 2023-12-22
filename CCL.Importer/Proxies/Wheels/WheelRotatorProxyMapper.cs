using CCL.Types.Proxies.Wheels;
using DV.Wheels;
using System.ComponentModel.Composition;

namespace CCL.Importer.Proxies.Wheels
{
    [Export(typeof(IProxyReplacer))]
    public class WheelRotationViaCodeReplacer : ProxyReplacer<WheelRotationViaCodeProxy, WheelRotationViaCode> { }

    [Export(typeof(IProxyReplacer))]
    public class PoweredWheelRotationViaAnimationReplacer : ProxyReplacer<PoweredWheelRotationViaAnimationProxy, PoweredWheelRotationViaAnimation> { }

    [Export(typeof(IProxyReplacer))]
    public class PoweredWheelRotationViaCodeReplacer : ProxyReplacer<PoweredWheelRotationViaCodeProxy, PoweredWheelRotationViaCode> { }
}
