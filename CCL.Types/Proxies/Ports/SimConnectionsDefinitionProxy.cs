using CCL.Types.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class SimConnectionsDefinitionProxy : MonoBehaviour, ICustomSerialized
    {
        [RenderMethodButtons]
        [MethodButton(nameof(PopulateComponents), "Populate Components")]
        [MethodButton("CCL.Creator.Wizards.SimulationEditorWindow:ShowWindow", "Connection Wizard")]
        public bool renderButtons;

        public List<SimComponentDefinitionProxy> executionOrder;

        public List<PortConnectionProxy> connections;

        [SerializeField, HideInInspector]
        private string connectionsJson;

        public List<PortReferenceConnectionProxy> portReferenceConnections;

        [SerializeField, HideInInspector]
        private string portReferenceConnectionsJson;

        public void OnValidate()
        {
            connectionsJson = JSONObject.ToJson(connections);
            portReferenceConnectionsJson = JSONObject.ToJson(portReferenceConnections);
        }

        public void AfterImport()
        {
            connections = JSONObject.FromJson(connectionsJson, () => new List<PortConnectionProxy>());
            portReferenceConnections = JSONObject.FromJson(portReferenceConnectionsJson, () => new List<PortReferenceConnectionProxy>());
        }

        
        public void PopulateComponents()
        {
            executionOrder ??= new List<SimComponentDefinitionProxy>();
            var allComponents = transform.root.GetComponentsInChildren<SimComponentDefinitionProxy>();

            foreach (var component in allComponents)
            {
                if (!executionOrder.Contains(component))
                {
                    executionOrder.Add(component);
                }
            }
        }

        public void DestroyConnectionsToComponent(SimComponentDefinitionProxy component)
        {
            string compId = component.ID;
            foreach (var connection in connections.ToArray())
            {
                if (component.ExposedPorts.Any(p => ConnectionUsesPort(connection, compId, p)))
                {
                    connections.Remove(connection);
                }
            }
            foreach (var connection in portReferenceConnections.ToArray())
            {
                if (component.ExposedPorts.Any(p => ConnectionUsesPort(connection, compId, p)) ||
                    component.ExposedPortReferences.Any(r => ConnectionUsesReference(connection, compId, r)))
                {
                    portReferenceConnections.Remove(connection);
                }
            }
            executionOrder?.Remove(component);
        }

        private static bool ConnectionUsesPort(PortConnectionProxy connection, string compId, PortDefinition port)
        {
            string fullId = $"{compId}.{port.ID}";
            return (connection.fullPortIdIn == fullId) || (connection.fullPortIdOut == fullId);
        }

        private static bool ConnectionUsesPort(PortReferenceConnectionProxy connection, string compId, PortDefinition port)
        {
            string fullId = $"{compId}.{port.ID}";
            return connection.portId == fullId;
        }

        private static bool ConnectionUsesReference(PortReferenceConnectionProxy connection, string compId, PortReferenceDefinition reference)
        {
            string fullId = $"{compId}.{reference.ID}";
            return connection.portReferenceId == fullId;
        }

        public void RemapComponentId(string oldId, string newId)
        {
            // don't remap if more than one component with same ID
            if (executionOrder.Count(c => c.ID == oldId) > 1)
            {
                return;
            }

            foreach (var connection in connections)
            {
                if (HasComponentId(connection.fullPortIdIn, oldId))
                {
                    ReplaceComponentId(ref connection.fullPortIdIn, newId);
                }
                if (HasComponentId(connection.fullPortIdOut, oldId))
                {
                    ReplaceComponentId(ref connection.fullPortIdOut, newId);
                }
            }

            foreach (var connection in portReferenceConnections)
            {
                if (HasComponentId(connection.portId, oldId))
                {
                    ReplaceComponentId(ref connection.portId, newId);
                }
                if (HasComponentId(connection.portReferenceId, oldId))
                {
                    ReplaceComponentId(ref connection.portReferenceId, newId);
                }
            }
        }

        private static bool HasComponentId(string fullPortId, string componentId)
        {
            if (string.IsNullOrWhiteSpace(fullPortId)) return false;
            var parts = fullPortId.Split('.');
            return (parts.Length == 2) && (componentId == parts[0]);
        }

        private static void ReplaceComponentId(ref string fullPortId, string newComponentId)
        {
            var parts = fullPortId.Split('.');
            fullPortId = $"{newComponentId}.{parts[1]}";
        }
    }

    [Serializable]
    public class PortConnectionProxy
    {
        [PortId(DVPortType.OUT)]
        public string fullPortIdOut;

        [PortId(DVPortType.IN)]
        public string fullPortIdIn;
    }

    [Serializable]
    public class PortReferenceConnectionProxy
    {
        [PortReferenceId]
        public string portReferenceId;

        [PortId]
        public string portId;
    }
}
