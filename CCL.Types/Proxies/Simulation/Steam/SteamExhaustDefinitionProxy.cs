using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Steam
{
    public class SteamExhaustDefinitionProxy : SimComponentDefinitionProxy, IS060Defaults, IS282Defaults
    {
        public float passiveExhaust = 0.6f;
        public float entrainmentRatio = 1.65f;

        [Header("Blower")]
        public float maxBlowerFlow = 2f;
        public float pressureForMaxBlowerFlow = 3f;

        [Header("Whistle")]
        public float maxWhistleFlow = 0.2f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "STEAM_CONSUMPTION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "AIR_FLOW"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "TOTAL_FLOW_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "WHISTLE_FLOW_NORMALIZED")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.MASS_RATE, "EXHAUST_FLOW", false),
            new PortReferenceDefinition(DVPortValueType.MASS_RATE, "ENGINE_MAX_FLOW", false),
            new PortReferenceDefinition(DVPortValueType.PRESSURE, "BOILER_PRESSURE", false),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "BLOWER_CONTROL", false),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "WHISTLE_CONTROL", false),
            new PortReferenceDefinition(DVPortValueType.CONTROL, "DAMPER_CONTROL", false)
        };

        #region Defaults

        public void ApplyS060Defaults()
        {
            passiveExhaust = 0.2f;
            entrainmentRatio = 1.65f;

            maxBlowerFlow = 0.5f;
            pressureForMaxBlowerFlow = 3.0f;

            maxWhistleFlow = 0.2f;
        }

        public void ApplyS282Defaults()
        {
            passiveExhaust = 0.6f;
            entrainmentRatio = 1.65f;

            maxBlowerFlow = 0.5f;
            pressureForMaxBlowerFlow = 3.0f;

            maxWhistleFlow = 0.2f;
        }

        #endregion
    }
}
