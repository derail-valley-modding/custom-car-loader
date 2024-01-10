using CCL.Importer.Types;
using CCL.Types.Proxies.Wheels;
using DV.Wheels;

namespace CCL.Importer.Proxies.Wheels
{
    [ProxyMap(typeof(WheelRotationViaAnimationProxy), typeof(WheelRotationViaAnimation))]
    [ProxyMap(typeof(WheelRotationViaCodeProxy), typeof(WheelRotationViaCode))]
    [ProxyMap(typeof(PoweredWheelRotationViaAnimationProxy), typeof(PoweredWheelRotationViaAnimation))]
    [ProxyMap(typeof(PoweredWheelRotationViaCodeProxy), typeof(PoweredWheelRotationViaCode))]
    [ProxyMap(typeof(PoweredWheelRotationViaAnimationProxy.AnimatorStartTimeOffsetPair), typeof(PoweredWheelRotationViaAnimation.AnimatorStartTimeOffsetPair))]
    [ProxyMap(typeof(PoweredWheelRotationViaCodeProxy.TransformRotationConfig), typeof(DV.TransformRotationConfig))]
    public class WheelRotationProxyMapper : ProxyReplacer { }
}
