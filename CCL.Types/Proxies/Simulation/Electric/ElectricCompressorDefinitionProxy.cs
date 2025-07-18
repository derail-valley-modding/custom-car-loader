﻿using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation.Electric
{
    [AddComponentMenu("CCL/Proxies/Simulation/Electric/Electric Compressor Definition Proxy")]
    public class ElectricCompressorDefinitionProxy : SimComponentDefinitionProxy, IHasFuseIdFields, IBE2Defaults
    {
        public float maxPower = 45000f;
        public float maxBarLiterProductionRate = 80f;
        public float activationPressureThreshold = 7f;
        public float mainReservoirVolume = 50f;
        public float smoothTime = 0.3f;

        [FuseId]
        public string powerFuseId = string.Empty;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.CONTROL, "ACTIVATION_SIGNAL_EXT_IN"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "COMPRESSOR_HEALTH_EXT_IN"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "POWER_CONSUMPTION"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "PRODUCTION_RATE"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.STATE, "PRODUCTION_RATE_NORMALIZED"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "MAIN_RES_VOLUME"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "ACTIVATION_PRESSURE_THRESHOLD")
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.VOLTS, "VOLTAGE_NORMALIZED", false)
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(powerFuseId), powerFuseId),
        };

        #region Defaults

        public void ApplyBE2Defaults()
        {
            maxPower = 10000.0f;
            maxBarLiterProductionRate = 15.0f;
            activationPressureThreshold = 8.3f;
            mainReservoirVolume = 500.0f;
            smoothTime = 0.3f;
        }

        #endregion
    }
}
