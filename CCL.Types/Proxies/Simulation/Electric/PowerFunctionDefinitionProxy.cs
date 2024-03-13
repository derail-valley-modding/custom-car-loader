using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation.Electric
{
    public class PowerFunctionDefinitionProxy : SimComponentDefinitionProxy, IBE2Defaults
    {
        public float multiplier = 1f;
        public float exponent = 1.2f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OUT")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.GENERIC, "IN", false)
        };

        #region Defaults

        public void ApplyBE2Defaults()
        {
            multiplier = 1.0f;
            exponent = 2.0f;
        }

        #endregion
    }
}
