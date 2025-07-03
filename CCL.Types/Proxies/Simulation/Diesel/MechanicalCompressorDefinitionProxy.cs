using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Diesel
{
    [AddComponentMenu("CCL/Proxies/Simulation/Diesel/Mechanical Compressor Definition Proxy")]
    public class MechanicalCompressorDefinitionProxy : SimComponentDefinitionProxy, IDE2Defaults, IDE6Defaults, IDH4Defaults, IDM3Defaults, IDM1UDefaults
    {
        public float loadTorque = 400f;
        public float maxProductionRate = 250f;
        public float activationPressureThreshold = 7f;
        public float mainReservoirVolume = 15f;
        public float smoothTime = 0.3f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "ACTIVATION_SIGNAL_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "COMPRESSOR_HEALTH_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TORQUE, "LOAD_TORQUE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "PRODUCTION_RATE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "PRODUCTION_RATE_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "MAIN_RES_VOLUME"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "ACTIVATION_PRESSURE_THRESHOLD")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.RPM, "ENGINE_RPM_NORMALIZED"),
        };

        #region Defaults

        public void ApplyDE2Defaults()
        {
            loadTorque = 200.0f;
            maxProductionRate = 50.0f;
            activationPressureThreshold = 8.3f;
            mainReservoirVolume = 1000.0f;
            smoothTime = 0.3f;
        }

        public void ApplyDE6Defaults()
        {
            loadTorque = 400.0f;
            maxProductionRate = 100.0f;
            activationPressureThreshold = 8.3f;
            mainReservoirVolume = 2000.0f;
            smoothTime = 0.3f;
        }

        public void ApplyDH4Defaults()
        {
            loadTorque = 270.0f;
            maxProductionRate = 75.0f;
            activationPressureThreshold = 8.3f;
            mainReservoirVolume = 800.0f;
            smoothTime = 0.3f;
        }

        public void ApplyDM3Defaults()
        {
            loadTorque = 250.0f;
            maxProductionRate = 45.0f;
            activationPressureThreshold = 8.3f;
            mainReservoirVolume = 500.0f;
            smoothTime = 0.3f;
        }

        public void ApplyDM1UDefaults()
        {
            loadTorque = 60.0f;
            maxProductionRate = 15.0f;
            activationPressureThreshold = 8.3f;
            mainReservoirVolume = 500.0f;
            smoothTime = 0.3f;
        }

        #endregion
    }
}