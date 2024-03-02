﻿using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation
{
    public class PassiveCoolerDefinitionProxy : SimComponentDefinitionProxy, IDE2Defaults, IDE6Defaults, IBE2Defaults
    {
        public float coolingRate = 12500f;
        
        [MethodButton(nameof(ApplyDE2Defaults), "Apply DE2 Defaults")]
        [MethodButton(nameof(ApplyDE6Defaults), "Apply DE6 Defaults")]
        [MethodButton(nameof(ApplyBE2Defaults), "Apply BE2 Defaults")]
        [RenderMethodButtons]
        public bool renderButtons;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.HEAT_RATE, "HEAT_OUT"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TEMPERATURE"),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TARGET_TEMPERATURE"),
        };

        public void ApplyDE2Defaults()
        {
            coolingRate = 100.0f;
        }

        public void ApplyDE6Defaults()
        {
            coolingRate = 250.0f;
        }

        public void ApplyBE2Defaults()
        {
            coolingRate = 6.0f;
        }
    }
}
