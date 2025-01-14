﻿using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies.Simulation
{
    public class SlugModuleProxy : MonoBehaviour, IHasPortIdFields, IHasFuseIdFields
    {
        [PortId(DVPortType.EXTERNAL_IN, DVPortValueType.VOLTS, true)]
        public string appliedVoltagePortId;
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.OHMS, true)]
        public string effectiveResistancePortId;
        [PortId(DVPortType.READONLY_OUT, DVPortValueType.AMPS, true)]
        public string totalAmpsPortId;
        [FuseId]
        public string powerFuseId;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(appliedVoltagePortId), appliedVoltagePortId, DVPortType.EXTERNAL_IN, DVPortValueType.VOLTS),
            new PortIdField(this, nameof(effectiveResistancePortId), effectiveResistancePortId, DVPortType.READONLY_OUT, DVPortValueType.OHMS),
            new PortIdField(this, nameof(totalAmpsPortId), totalAmpsPortId, DVPortType.READONLY_OUT, DVPortValueType.AMPS),
        };

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(powerFuseId), powerFuseId)
        };
    }
}
