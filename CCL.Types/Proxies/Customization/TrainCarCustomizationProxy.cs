using CCL.Types.Json;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Diesel;
using CCL.Types.Proxies.Simulation.Electric;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Customization
{
    public class TrainCarCustomizationProxy : MonoBehaviour, ICustomSerialized, IHasFuseIdFields
    {
        public enum STDSimPort : byte
        {
            WheelSpeedKMH = 0,
            TractionMotorAmps = 1,
            TractionMotorAmpLimit = 2,
            TractionMotorAmpLimitEffect = 3,
            Temperature = 4,
            TractionMotorAmpsMax = 5,
            EngineRPM = 6,
            EngineRPMMax = 7,
            TurbineRPM = 8,
            TurbineRPMMax = 9,
            Fuel = 10,
            FuelMax = 11,
            Oil = 12,
            OilMax = 13,
            Sand = 14,
            SandMax = 15,
            EngineOn = 16,
            FuelLampState = 100,
            OilLampState = 101,
            SandLampState = 102,
            SanderLampState = 103,
            WipersLampState = 104,
            HeadlightFLampState = 105,
            HeadlightRLampState = 106,
            CabLightLampState = 107,
            EngineRpmLampState = 120,
            AmpsLampState = 121
        }

        [Serializable, NotProxied]
        public class STDPortDefinition
        {
            public STDSimPort port;
            [PortId(null, null, false)]
            public string name = string.Empty;
            public bool readOnly = true;
        }

        [FuseId]
        public string electronicsFuseID = string.Empty;
        public STDPortDefinition[] Ports = new STDPortDefinition[0];

        [SerializeField, HideInInspector]
        private string? _json;
        [SerializeField]
        [RenderMethodButtons]
        [MethodButton(nameof(AutoSetup), "Auto Setup")]
        private bool _buttons;

        public IEnumerable<FuseIdField> ExposedFuseIdFields => new[]
        {
            new FuseIdField(this, nameof(electronicsFuseID), electronicsFuseID)
        };

        public void OnValidate()
        {
            _json = JSONObject.ToJson(Ports);
        }

        public void AfterImport()
        {
            Ports = JSONObject.FromJson(_json, () => new STDPortDefinition[0]);
        }

        public void AutoSetup()
        {
            var alreadySet = Ports.Select(x => x.port);
            var list = Ports.ToList();
            var connections = GetComponentInChildren<SimConnectionsDefinitionProxy>();
            var flagPowertrain = false;
            var flagLamps = false;

            if (connections == null)
            {
                Debug.LogError("No Simulation found, cannot set customisation ports");
                return;
            }

            TryAddPortForComp<TractionDefinitionProxy>(STDSimPort.WheelSpeedKMH, "WHEEL_SPEED_KMH_EXT_IN");
            TryAddPortForComp<TractionMotorSetDefinitionProxy>(STDSimPort.TractionMotorAmps, "AMPS_PER_TM");
            TryAddPortForComp<TractionMotorSetDefinitionProxy>(STDSimPort.TractionMotorAmpsMax, "MAX_AMPS_PER_TM");
            TryAddPortForComp<TractionGeneratorDefinitionProxy>(STDSimPort.TractionMotorAmpLimit, "EXTERNAL_CURRENT_LIMIT_EXT_IN");
            TryAddPortForComp<TractionGeneratorDefinitionProxy>(STDSimPort.TractionMotorAmpLimitEffect, "EXTERNAL_CURRENT_LIMIT_ACTIVE", readOnly: false);
            TryAddPortForComp<HeatReservoirDefinitionProxy>(STDSimPort.Temperature, "TEMPERATURE");
            TryAddPortForComp<DieselEngineDirectDefinitionProxy>(STDSimPort.EngineOn, "ENGINE_ON");
            TryAddPortForComp<DieselEngineDirectDefinitionProxy>(STDSimPort.EngineRPM, "RPM");
            TryAddPortForComp<DieselEngineDirectDefinitionProxy>(STDSimPort.EngineRPMMax, "MAX_RPM");
            TryAddPortForComp<HydraulicTransmissionDefinitionProxy>(STDSimPort.TurbineRPM, "TURBINE_RPM");
            flagPowertrain |= TryAddPortForComp<DieselEngineDirectDefinitionProxy>(STDSimPort.TurbineRPMMax, "MAX_RPM");
            TryAddPortForComp<ResourceContainerProxy>(STDSimPort.Fuel, "AMOUNT", x => x.type == ResourceContainerType.Fuel);
            TryAddPortForComp<ResourceContainerProxy>(STDSimPort.FuelMax, "CAPACITY", x => x.type == ResourceContainerType.Fuel);
            TryAddPortForComp<ResourceContainerProxy>(STDSimPort.Fuel, "AMOUNT", x => x.type == ResourceContainerType.ElectricCharge);
            TryAddPortForComp<ResourceContainerProxy>(STDSimPort.FuelMax, "CAPACITY", x => x.type == ResourceContainerType.ElectricCharge);
            TryAddPortForComp<ResourceContainerProxy>(STDSimPort.Oil, "AMOUNT", x => x.type == ResourceContainerType.Oil);
            TryAddPortForComp<ResourceContainerProxy>(STDSimPort.OilMax, "CAPACITY", x => x.type == ResourceContainerType.Oil);
            TryAddPortForComp<ResourceContainerProxy>(STDSimPort.Sand, "AMOUNT", x => x.type == ResourceContainerType.Sand);
            TryAddPortForComp<ResourceContainerProxy>(STDSimPort.SandMax, "CAPACITY", x => x.type == ResourceContainerType.Sand);

            PortReferenceConnectionProxy connection;

            var controls = GetComponentsInChildren<OverridableControlProxy>();

            foreach (var control in controls)
            {
                switch (control.ControlType)
                {
                    case OverridableControlType.Sander:
                        if (TryGetPortRefConnection(control.portId, out connection))
                        {
                            TryAddPortForComp<LampLogicDefinitionProxy>(STDSimPort.SanderLampState, "LAMP_STATE",
                                x => x.GetFullReaderPortRefId() == connection.portReferenceId);
                        }
                        break;
                    case OverridableControlType.HeadlightsFront:
                        if (TryGetPortRefConnection(control.portId, out connection))
                        {
                            TryAddPortForComp<LampLogicDefinitionProxy>(STDSimPort.HeadlightFLampState, "LAMP_STATE",
                                x => x.GetFullReaderPortRefId() == connection.portReferenceId);
                        }
                        break;
                    case OverridableControlType.HeadlightsRear:
                        if (TryGetPortRefConnection(control.portId, out connection))
                        {
                            TryAddPortForComp<LampLogicDefinitionProxy>(STDSimPort.HeadlightRLampState, "LAMP_STATE",
                                x => x.GetFullReaderPortRefId() == connection.portReferenceId);
                        }
                        break;
                    case OverridableControlType.Wipers:
                        if (TryGetPortRefConnection(control.portId, out connection))
                        {
                            TryAddPortForComp<LampLogicDefinitionProxy>(STDSimPort.WipersLampState, "LAMP_STATE",
                                x => x.GetFullReaderPortRefId() == connection.portReferenceId);
                        }
                        break;
                    case OverridableControlType.CabLight:
                    case OverridableControlType.IndCabLight:
                        if (TryGetPortRefConnection(control.portId, out connection))
                        {
                            TryAddPortForComp<LampLogicDefinitionProxy>(STDSimPort.CabLightLampState, "LAMP_STATE",
                                x => x.GetFullReaderPortRefId() == connection.portReferenceId);
                        }
                        break;
                    default:
                        continue;
                }
            }

            var resources = GetComponentsInChildren<ResourceContainerProxy>();

            foreach (var resource in resources)
            {
                switch (resource.type)
                {
                    case ResourceContainerType.Fuel:
                    case ResourceContainerType.ElectricCharge:
                        if (TryGetPortRefConnection(resource.GetFullPortId("NORMALIZED"), out connection))
                        {
                            TryAddPortForComp<LampLogicDefinitionProxy>(STDSimPort.FuelLampState, "LAMP_STATE",
                                x => x.GetFullReaderPortRefId() == connection.portReferenceId);
                        }
                        break;
                    case ResourceContainerType.Oil:
                        if (TryGetPortRefConnection(resource.GetFullPortId("NORMALIZED"), out connection))
                        {
                            TryAddPortForComp<LampLogicDefinitionProxy>(STDSimPort.OilLampState, "LAMP_STATE",
                                x => x.GetFullReaderPortRefId() == connection.portReferenceId);
                        }
                        break;
                    case ResourceContainerType.Sand:
                        if (TryGetPortRefConnection(resource.GetFullPortId("NORMALIZED"), out connection))
                        {
                            TryAddPortForComp<LampLogicDefinitionProxy>(STDSimPort.SandLampState, "LAMP_STATE",
                                x => x.GetFullReaderPortRefId() == connection.portReferenceId);
                        }
                        break;
                    default:
                        continue;
                }
            }

            if (TryGetSimComponent(null, out DieselEngineDirectDefinitionProxy engine) &&
                TryGetPortRefConnection(engine.GetFullPortId("RPM_NORMALIZED"), out connection))
            {
                flagLamps |= !TryAddPortForComp<LampLogicDefinitionProxy>(STDSimPort.EngineRpmLampState, "LAMP_STATE",
                    x => x.GetFullReaderPortRefId() == connection.portReferenceId);
            }

            if (TryGetSimComponent(null, out TractionMotorSetDefinitionProxy tm) &&
                TryGetPortRefConnection(tm.GetFullPortId("AMPS_PER_TM"), out connection))
            {
                flagLamps |= !TryAddPortForComp<LampLogicDefinitionProxy>(STDSimPort.AmpsLampState, "LAMP_STATE",
                    x => x.GetFullReaderPortRefId() == connection.portReferenceId);
            }

            Ports = list.ToArray();

            if (flagPowertrain)
            {
                Debug.LogWarning("Some ports may have been assigned for the wrong powertrain.");
            }

            if (flagLamps)
            {
                Debug.LogWarning("Some lamps may have not been assigned.");
            }

            bool TryAddPortForComp<T>(STDSimPort port, string id, Predicate<T>? condition = null, bool readOnly = true)
                where T : SimComponentDefinitionProxy
            {
                if (alreadySet.Contains(port)) return false;

                if (TryGetSimComponent(condition, out T comp))
                {
                    list.Add(new STDPortDefinition
                    {
                        port = port,
                        name = comp.GetFullPortId(id),
                        readOnly = readOnly
                    });

                    return true;
                }

                return false;
            }

            bool TryGetSimComponent<T>(Predicate<T>? condition, out T matched)
                where T : SimComponentDefinitionProxy
            {
                foreach (SimComponentDefinitionProxy comp in connections.executionOrder)
                {
                    if (comp is T match && (condition == null || condition(match)))
                    {
                        matched = match;
                        return true;
                    }
                }

                matched = null!;
                return false;
            }

            bool TryGetPortRefConnection(string id, out PortReferenceConnectionProxy connection)
            {
                foreach (var item in connections.portReferenceConnections)
                {
                    if (item.portId == id)
                    {
                        connection = item;
                        return true;
                    }
                }

                connection = null!;
                return false;
            }
        }
    }
}
