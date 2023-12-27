using CCL.Importer.Types;
using CCL.Types.Proxies.Wheels;
using DV.Wheels;
using System.ComponentModel.Composition;

namespace CCL.Importer.Proxies.Wheels
{
    [ProxyMap(typeof(WheelRotationViaAnimationProxy), typeof(WheelRotationViaAnimation))]
    [ProxyMap(typeof(WheelRotationViaCodeProxy), typeof(WheelRotationViaCode))]
    [ProxyMap(typeof(PoweredWheelRotationViaAnimationProxy), typeof(PoweredWheelRotationViaAnimation))]
    [ProxyMap(typeof(PoweredWheelRotationViaCodeProxy), typeof(PoweredWheelRotationViaCode))]
    public class WheelRotationProxyMapper : ProxyReplacer { }
}
