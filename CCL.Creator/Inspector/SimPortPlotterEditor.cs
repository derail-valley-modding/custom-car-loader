using CCL.Creator.Utility;
using CCL.Types;
using CCL.Types.Components;
using CCL.Types.Proxies.Ports;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(SimPortPlotter))]
    internal class SimPortPlotterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (GUILayout.Button("Auto setup"))
            {
                var plotter = (SimPortPlotter)target;
                var root = plotter.transform.root;

                var connections = root.gameObject.GetComponentInChildren<SimConnectionsDefinitionProxy>();

                if (connections != null)
                {
                    var ports = plotter.PortIds.ToList();
                    var refs = plotter.PortReferenceIds.ToList();

                    foreach (var item in connections.executionOrder)
                    {
                        if (item is IRecommendedDebugPorts dPorts)
                        {
                            ports.AddRange(dPorts.GetDebugPorts().Select(x => $"{item.ID}.{x}"));
                        }

                        if (item is IRecommendedDebugPortReferences dPortRefs)
                        {
                            refs.AddRange(dPortRefs.GetDebugPortReferences().Select(x => $"{item.ID}.{x}"));
                        }
                    }

                    plotter.PortIds = ports.Distinct().ToList();
                    plotter.PortReferenceIds = refs.Distinct().ToList();

                    AssetHelper.SaveAsset(target);
                }
            }
        }
    }
}
