using CCL.Types;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;

using UObject = UnityEngine.Object;

namespace CCL.Creator.Validators
{
    [RequiresStep(typeof(LiverySettingsValidator))]
    internal class SimulationValidator : LiveryValidator
    {
        public override string TestName => "Simulation";

        protected override ValidationResult ValidateLivery(CustomCarVariant livery)
        {
            var result = Pass();

            var connectionDef = livery.prefab!.GetComponentInChildren<SimConnectionsDefinitionProxy>();
            if (!connectionDef) return Skip();

            if (connectionDef.executionOrder.Any(x => x == null))
            {
                result.Fail("Execution order has null entries");
                return result;
            }

            var components = GetAllOfType<SimComponentDefinitionProxy>(livery).ToList();
            var havePortIds = GetAllOfType<IHasPortIdFields>(livery).ToList();
            var haveFuseIds = GetAllOfType<IHasFuseIdFields>(livery).ToList();

            var compIds = new HashSet<string>();
            var portIds = new HashSet<string>();

            // Check for duplicate component IDs or port IDs.
            foreach (var component in components)
            {
                if (compIds.Contains(component.ID))
                {
                    result.Fail($"Duplicate component ID {component.ID}");
                }
                else
                {
                    compIds.Add(component.ID);
                }

                foreach (var port in component.ExposedPorts)
                {
                    var fullId = component.GetFullPortId(port.ID);

                    if (portIds.Contains(fullId))
                    {
                        result.Fail($"Duplicate port ID {fullId}");
                    }
                    else
                    {
                        portIds.Add(fullId);
                    }
                }
            }

            // Check port connections.
            foreach (var connection in connectionDef.connections)
            {
                if (!CheckPortExists(components, connection.fullPortIdOut) || !CheckPortExists(components, connection.fullPortIdIn))
                {
                    result.Warning($"Invalid port connection \"{connection.fullPortIdOut}\"->\"{connection.fullPortIdIn}\"", connectionDef);
                }
            }

            foreach (var connection in connectionDef.portReferenceConnections)
            {
                if (!CheckPortExists(components, connection.portId) || !CheckPortReferenceExists(components, connection.portReferenceId))
                {
                    if (string.IsNullOrEmpty(connection.portId))
                    {
                        result.Warning($"Empty ref connection \"{connection.portReferenceId}\"", connectionDef);
                    }
                    else
                    {
                        result.Warning($"Invalid ref connection \"{connection.portReferenceId}\"->\"{connection.portId}\"", connectionDef);
                    }
                }
            }

            // Check port/fuse ID fields.
            foreach (var hasPortId in havePortIds)
            {
                foreach (var field in hasPortId.ExposedPortIdFields)
                {
                    if (!field.IsAssigned)
                    {
                        if (!field.IsMultiValue && field.Required)
                        {
                            result.Fail($"Port field {field.FullName} must be assigned", hasPortId is UObject obj ? obj : null);
                        }

                        continue;
                    }

                    foreach (string id in field.AssignedIds!)
                    {
                        if (!CheckPortExists(components, id))
                        {
                            result.Warning($"Port field {field.FullName} target \"{id}\" was not found",
                                hasPortId is UObject obj ? obj : null);
                        }
                    }
                }
            }

            foreach (var hasFuseId in haveFuseIds)
            {
                foreach (var field in hasFuseId.ExposedFuseIdFields)
                {
                    if (!field.IsAssigned)
                    {
                        if (!field.IsMultiValue && field.Required)
                        {
                            result.Fail($"Fuse field {field.FullName} must be assigned", hasFuseId is UObject obj ? obj : null);
                        }

                        continue;
                    }

                    foreach (string id in field.AssignedIds!)
                    {
                        if (!CheckFuseExists(components, id))
                        {
                            result.Warning($"Fuse field {field.FullName} target \"{id}\" was not found",
                                hasFuseId is UObject obj ? obj : null);
                        }
                    }
                }
            }

            // Check control definitions.
            var controls = GetAllOfType<ExternalControlDefinitionProxy>(livery)
                .Cast<SimComponentDefinitionProxy>()
                .Concat(GetAllOfType<GenericControlDefinitionProxy>(livery));

            var feeders = GetAllOfType<InteractablePortFeederProxy>(livery);

            foreach (var externalControl in controls)
            {
                if (!CheckPortFeederExists(feeders, $"{externalControl.ID}.EXT_IN"))
                {
                    result.Warning($"Control \"{externalControl.ID}\" has no interactable port feeder", externalControl);
                }
            }

            foreach (var gearshift in GetAllOfType<ManualTransmissionInputDefinitionProxy>(livery))
            {
                if (!CheckPortFeederExists(feeders, $"{gearshift.ID}.CONTROL_EXT_IN"))
                {
                    result.Warning($"Control \"{gearshift.ID}\" has no interactable port feeder", gearshift);
                }
            }

            // Check fuse feeders.
            var fuseFeeders = GetAllOfType<InteractableFuseFeederProxy>(livery);

            foreach (var hasFuses in components)
            {
                foreach (var fuse in hasFuses.ExposedFuses)
                {
                    string fullId = $"{hasFuses.ID}.{fuse.id}";
                    if (!CheckFuseFeederExists(fuseFeeders, fullId))
                    {
                        result.Warning($"Fuse \"{fullId}\" has no interactable fuse feeder", hasFuses);
                    }
                }
            }

            // Check lamps.
            foreach(var lamp in GetAllOfType<LampLogicDefinitionProxy>(livery))
            {
                // Lamps MUST be connected.
                if (!connectionDef.portReferenceConnections.TryFind(x => x.portReferenceId == lamp.GetFullPortId(lamp.inputReader.ID), out var connection)
                    || string.IsNullOrEmpty(connection.portId))
                {
                    result.Fail($"Lamp \"{lamp.ID}\" is not connected", lamp);
                }
            }

            // Check brakes.
            var brakeSetup = livery.parentType!.brakes;
            if (livery.prefab.GetComponent<CompressorSimControllerProxy>())
            {
                if (!brakeSetup.hasCompressor)
                {
                    result.Warning("Prefab has compressor component, but car's brake config says it does not", livery);
                }
            }

            var overridableControls = GetAllOfType<OverridableControlProxy>(livery);
            if (overridableControls.Any(c => c.ControlType == OverridableControlType.TrainBrake))
            {
                if (brakeSetup.brakeValveType == CustomCarType.BrakesSetup.TrainBrakeType.None)
                {
                    result.Warning("Prefab has train brake control, but car's brake config has no valve type set", livery);
                }
            }
            if (overridableControls.Any(c => c.ControlType == OverridableControlType.IndBrake))
            {
                if (!brakeSetup.hasIndependentBrake)
                {
                    result.Warning("Prefab has independent brake control, but car's brake config says it does not", livery);
                }
            }

            return result;
        }

        private bool CheckIdExists(IEnumerable<SimComponentDefinitionProxy> components, string fullId, Func<SimComponentDefinitionProxy, string, bool> predicate)
        {
            if (string.IsNullOrWhiteSpace(fullId)) return false;

            string[] parts = fullId.Split(".".ToCharArray(), 2);
            if (parts.Length != 2) return false;

            foreach (var component in components)
            {
                if (component.ID == parts[0])
                {
                    if (predicate(component, parts[1]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CompHasPort(SimComponentDefinitionProxy c, string id) => c.ExposedPorts.Any(p => p.ID == id);

        private bool CheckPortExists(IEnumerable<SimComponentDefinitionProxy> components, string fullId) =>
            CheckIdExists(components, fullId, CompHasPort);


        private bool CompHasRef(SimComponentDefinitionProxy c, string id) => c.ExposedPortReferences.Any(r => r.ID == id);

        private bool CheckPortReferenceExists(IEnumerable<SimComponentDefinitionProxy> components, string fullId) =>
            CheckIdExists(components, fullId, CompHasRef);


        private bool CompHasFuse(SimComponentDefinitionProxy c, string id) => c.ExposedFuses.Any(f => f.id == id);

        private bool CheckFuseExists(IEnumerable<SimComponentDefinitionProxy> components, string fullId) =>
            CheckIdExists(components, fullId, CompHasFuse);


        private IEnumerable<T> GetAllOfType<T>(CustomCarVariant livery)
        {
            IEnumerable<T> result = livery.prefab!.GetComponentsInChildren<T>();

            if (livery.interiorPrefab)
            {
                result = result.Concat(livery.interiorPrefab!.GetComponentsInChildren<T>());
            }

            if (livery.externalInteractablesPrefab)
            {
                result = result.Concat(livery.externalInteractablesPrefab!.GetComponentsInChildren<T>());
            }
            return result;
        }

        private bool CheckPortFeederExists(IEnumerable<InteractablePortFeederProxy> feeders, string controlPortId)
        {
            return feeders.Any(pf => pf.portId == controlPortId);
        }

        private bool CheckFuseFeederExists(IEnumerable<InteractableFuseFeederProxy> feeders, string fuseId)
        {
            return feeders.Any(ff => ff.fuseId == fuseId);
        }
    }
}
