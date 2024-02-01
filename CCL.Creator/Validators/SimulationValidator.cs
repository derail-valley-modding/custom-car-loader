using CCL.Types;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (!connectionDef) return Pass();

            var components = GetAllOfType<SimComponentDefinitionProxy>(livery).ToList();
            var havePortIds = GetAllOfType<IHasPortIdFields>(livery).ToList();
            var haveFuseIds = GetAllOfType<IHasFuseIdFields>(livery).ToList();

            // Check port connections
            foreach (var connection in connectionDef.connections)
            {
                if (!CheckPortExists(components, connection.fullPortIdOut) || !CheckPortExists(components, connection.fullPortIdIn))
                {
                    result.Warning($"Invalid port connection \"{connection.fullPortIdOut}\"->\"{connection.fullPortIdIn}\"");
                }
            }

            foreach (var connection in connectionDef.portReferenceConnections)
            {
                if (!CheckPortExists(components, connection.portId) || !CheckPortReferenceExists(components, connection.portReferenceId))
                {
                    result.Warning($"Invalid ref connection \"{connection.portId}\"->\"{connection.portReferenceId}\"");
                }
            }

            // Check port/fuse ID fields
            foreach (var hasPortId in havePortIds)
            {
                foreach (var field in hasPortId.ExposedPortIdFields.Where(f => f.IsAssigned))
                {
                    foreach (string id in field.AssignedIds!)
                    {
                        if (!CheckPortExists(components, id))
                        {
                            result.Warning($"Port field {field.FullName} target \"{id}\" was not found");
                        }
                    }
                }
            }

            foreach (var hasFuseId in haveFuseIds)
            {
                foreach (var field in hasFuseId.ExposedFuseIdFields.Where(f => f.IsAssigned))
                {
                    foreach (string id in field.AssignedIds!)
                    {
                        if (!CheckFuseExists(components, id))
                        {
                            result.Warning($"Fuse field {field.FullName} target \"{id}\" was not found");
                        }
                    }
                }
            }

            // Check control definitions
            var controls = GetAllOfType<ExternalControlDefinitionProxy>(livery)
                .Cast<SimComponentDefinitionProxy>()
                .Concat(GetAllOfType<GenericControlDefinitionProxy>(livery));

            var feeders = GetAllOfType<InteractablePortFeederProxy>(livery);

            foreach (var externalControl in controls)
            {
                if (!CheckPortFeederExists(feeders, $"{externalControl.ID}.EXT_IN"))
                {
                    result.Warning($"Control \"{externalControl.ID}\" has no interactable port feeder");
                }
            }

            foreach (var gearshift in GetAllOfType<ManualTransmissionInputDefinitionProxy>(livery))
            {
                if (!CheckPortFeederExists(feeders, $"{gearshift.ID}.CONTROL_EXT_IN"))
                {
                    result.Warning($"Control \"{gearshift.ID}\" has no interactable port feeder");
                }
            }

            // Check fuse feeders
            var fuseFeeders = GetAllOfType<InteractableFuseFeederProxy>(livery);

            foreach (var hasFuses in components)
            {
                foreach (var fuse in hasFuses.ExposedFuses)
                {
                    string fullId = $"{hasFuses.ID}.{fuse.id}";
                    if (!CheckFuseFeederExists(fuseFeeders, fullId))
                    {
                        result.Warning($"Fuse \"{fullId}\" has no interactable fuse feeder");
                    }
                }
            }

            // Check brakes
            var brakeSetup = livery.parentType!.brakes;

            if (livery.prefab.GetComponent<CompressorSimControllerProxy>())
            {
                if (!brakeSetup.hasCompressor)
                {
                    result.Warning("Prefab has compressor component, but car's brake config says it does not");
                }
            }

            var overridableControls = GetAllOfType<OverridableControlProxy>(livery);
            if (overridableControls.Any(c => c.ControlType == OverridableControlType.TrainBrake))
            {
                if (brakeSetup.brakeValveType == CustomCarType.BrakesSetup.TrainBrakeType.None)
                {
                    result.Warning("Prefab has train brake control, but car's brake config has no valve type set");
                }
            }

            if (overridableControls.Any(c => c.ControlType == OverridableControlType.IndBrake))
            {
                if (!brakeSetup.hasIndependentBrake)
                {
                    result.Warning("Prefab has independent brake control, but car's brake config says it does not");
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
