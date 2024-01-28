using CCL.Types.Proxies.Ports;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Diesel
{
    public class MechanicalCompressorDefinitionProxy : SimComponentDefinitionProxy
    {
        public float loadTorque = 400f;
        public float maxProductionRate = 250f;
        public float activationPressureThreshold = 7f;
        public float mainReservoirVolume = 15f;
        public float smoothTime = 0.3f;

        [MethodButton(nameof(ApplyDM3Defaults), "Apply DM3 Defaults")]
        [RenderMethodButtons]
        public bool renderButtons;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "ACTIVATION_SIGNAL_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "COMPRESSOR_HEALTH_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TORQUE, "LOAD_TORQUE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "PRODUCTION_RATE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "PRODUCTION_RATE_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "MAIN_RES_VOLUME"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "ACTIVATION_PRESSURE_THRESHOLD"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "IS_POWERED"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.RPM, "ENGINE_RPM_NORMALIZED"),
        };

        public void ApplyDM3Defaults()
        {
            loadTorque = 250;
            maxProductionRate = 45;
            activationPressureThreshold = 7;
            mainReservoirVolume = 50;
            smoothTime = 0.3f;
        }
    }
}