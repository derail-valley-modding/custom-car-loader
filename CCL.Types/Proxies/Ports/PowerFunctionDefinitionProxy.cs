using System.Collections.Generic;

namespace CCL.Types.Proxies.Ports
{
    public class PowerFunctionDefinitionProxy : SimComponentDefinitionProxy, IBE2Defaults
    {
        public float multiplier = 1f;
        public float exponent = 1.2f;

        public PortReferenceDefinition input = new PortReferenceDefinition(DVPortValueType.GENERIC, "IN");
        public PortDefinition output = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OUT");

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            output
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            input
        };

        #region Defaults

        public void ApplyBE2Defaults()
        {
            multiplier = 1.0f;
            exponent = 1.0f;
        }

        #endregion
    }
}
