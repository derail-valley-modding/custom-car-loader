using CCL.Types.Proxies.Ports;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Diesel
{
    public class CompressorDieselEngineDefinitionProxy : SimComponentDefinitionProxy
    {
        public float maxPower = 60000f;
        public float idleBarLiterProduction = 10f;
        public float activationPressureThreshold = 7f;
        public float mainReservoirVolume = 15f;
        public float smoothTime = 0.3f;

        [FuseId]
        public string powerFuseId;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "ACTIVATION_SIGNAL_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "COMPRESSOR_HEALTH_EXT_IN"),
            new PortDefinition(DVPortType.IN, DVPortValueType.POWER, "POWER_IN"),
            new PortDefinition(DVPortType.OUT, DVPortValueType.POWER, "POWER_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "PRODUCTION_RATE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "PRODUCTION_RATE_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "MAIN_RES_VOLUME"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "ACTIVATION_PRESSURE_THRESHOLD"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "IS_POWERED"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.RPM, "ENGINE_RPM_NORMALIZED"),
            new PortReferenceDefinition(DVPortValueType.RPM, "ENGINE_IDLE_RPM_NORMALIZED"),
        };
    }
}