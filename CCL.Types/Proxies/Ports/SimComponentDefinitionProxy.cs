using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    [ExecuteAlways]
    public abstract class SimComponentDefinitionProxy : MonoBehaviour
    {
        private string? _previousId = null;
        [Delayed]
        public string ID;

        public virtual IEnumerable<PortDefinition> ExposedPorts => Enumerable.Empty<PortDefinition>();

        public virtual IEnumerable<PortReferenceDefinition> ExposedPortReferences => Enumerable.Empty<PortReferenceDefinition>();

        public virtual IEnumerable<FuseDefinition> ExposedFuses => Enumerable.Empty<FuseDefinition>();

        void Reset()
        {
            if (string.IsNullOrWhiteSpace(ID) && (GetComponents<SimComponentDefinitionProxy>().Length <= 1))
            {
                ID = gameObject.name;
            }

            var connections = transform.root.GetComponentInChildren<SimConnectionsDefinitionProxy>();
            if (connections)
            {
                if ((connections.executionOrder == null) || !connections.executionOrder.Contains(this))
                {
                    connections.executionOrder ??= new List<SimComponentDefinitionProxy>();
                    connections.executionOrder.Add(this);
                }
            }
        }

        void OnDestroy()
        {
            var connections = transform.root.GetComponentInChildren<SimConnectionsDefinitionProxy>();
            if (connections)
            {
                connections.DestroyConnectionsToComponent(this);
            }
        }

        void OnValidate()
        {
            if (!string.IsNullOrWhiteSpace(_previousId) && !string.IsNullOrWhiteSpace(ID) && (ID != _previousId))
            {
                var connections = transform.root.GetComponentInChildren<SimConnectionsDefinitionProxy>();
                if (connections)
                {
                    connections.RemapComponentId(_previousId!, ID);
                }
            }
            _previousId = ID;
        }
    }
}