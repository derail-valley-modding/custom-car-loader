using CCL.Types.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class SimConnectionsDefinitionProxy : MonoBehaviour, ICustomSerialized
    {
        public SimComponentDefinitionProxy[] executionOrder;

        public List<PortConnectionProxy> connections;

        [SerializeField, HideInInspector]
        private string connectionsJson;

        public List<PortReferenceConnectionProxy> portReferenceConnections;

        [SerializeField, HideInInspector]
        private string portReferenceConnectionsJson;

        [RenderMethodButtons]
        [MethodButton(nameof(PopulateComponents), "Populate Components")]
        [MethodButton("CCL.Creator.Editor.SimulationEditorWindow:ShowWindow", "Launch Wizard")]
        public bool renderButtons;

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
            var newExecutionList = executionOrder?.ToList() ?? new List<SimComponentDefinitionProxy>();
            var allComponents = transform.root.GetComponentsInChildren<SimComponentDefinitionProxy>();

            foreach (var component in allComponents)
            {
                if (!newExecutionList.Contains(component))
                {
                    newExecutionList.Add(component);
                }
            }

            executionOrder = newExecutionList.ToArray();
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
