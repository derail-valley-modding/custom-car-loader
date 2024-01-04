using CCL.Types.Proxies.Ports;

namespace CCL.Types.Proxies.Controllers
{
    public abstract class PoweredControlHandlerBase
    {
        [PortId(DVPortValueType.CONTROL, false)]
        public string controlId;

        [FuseId]
        public string powerFuseId;
    }
}
