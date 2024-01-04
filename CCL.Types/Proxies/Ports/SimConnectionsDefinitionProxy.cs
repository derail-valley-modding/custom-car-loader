using CCL.Types.Json;
using System;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class SimConnectionsDefinitionProxy : MonoBehaviour, ICustomSerialized
    {
        public SimComponentDefinitionProxy[] executionOrder;

        public PortConnectionProxy[] connections;

        [SerializeField, HideInInspector]
        private string connectionsJson;

        public PortReferenceConnectionProxy[] portReferenceConnections;

        [SerializeField, HideInInspector]
        private string portReferenceConnectionsJson;

        [RenderMethodButtons]
        [MethodButton("CCL.Types.Proxies.Ports.SimConnectionsDefinitionProxy:PopulateComponents", "Populate Components")]
        public bool renderButtons;

        public void OnValidate()
        {
            connectionsJson = JSONObject.ToJson(connections);
            portReferenceConnectionsJson = JSONObject.ToJson(portReferenceConnections);
        }

        public void AfterImport()
        {
            connections = JSONObject.FromJson<PortConnectionProxy[]>(connectionsJson);
            portReferenceConnections = JSONObject.FromJson<PortReferenceConnectionProxy[]>(portReferenceConnectionsJson);
        }

        
        public void PopulateComponents()
        {
            var newExecutionList = executionOrder.ToList();
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
        [PortId(new DVPortType[] { DVPortType.OUT })]
        public string fullPortIdOut;

        [PortId(new DVPortType[] { DVPortType.IN })]
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
