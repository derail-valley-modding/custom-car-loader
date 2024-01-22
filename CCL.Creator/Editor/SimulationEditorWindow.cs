using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Editor
{
    public class SimulationEditorWindow : EditorWindow
    {
        private static SimulationEditorWindow? _instance;

        public static void ShowWindow(SimConnectionsDefinitionProxy simConnections)
        {
            _instance = GetWindow<SimulationEditorWindow>();
            _instance.SimConnections = simConnections;
            _instance.titleContent = new GUIContent("CCL - Simulation Setup");

            simConnections.connections ??= new List<PortConnectionProxy>();
            simConnections.portReferenceConnections ??= new List<PortReferenceConnectionProxy>();

            _instance.Show();
        }

        public SimConnectionsDefinitionProxy SimConnections = null!;
        private Vector2 _scrollPosition = Vector2.zero;

        public void OnGUI()
        {
            var componentInfos = SimConnections.executionOrder?.Select(c => new ComponentInfo(c)).ToList() ?? new List<ComponentInfo>(0);

            Rect canvas = new Rect(0, 0, position.width - (H_PADDING * 2), componentInfos.Sum(c => c.PaddedHeight));
            _scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), _scrollPosition, canvas);

            float compBoxWidth = canvas.width - (H_PADDING * 2.5f);
            float columnWidth = compBoxWidth / 3;
            float entryWidth = columnWidth - (H_PADDING * 2);

            float yOffset = 0;
            foreach (var component in componentInfos)
            {
                yOffset += H_PADDING;

                var compBox = new Rect(H_PADDING, yOffset, compBoxWidth, component.Height);
                GUI.BeginGroup(compBox, $"{component.Component.ID} ({component.Component.GetType().Name.Replace("Proxy", string.Empty)})", "box");
                yOffset -= LINE_HEIGHT / 2f;
                float boxStartY = yOffset;

                foreach (var port in component.Ports)
                {
                    yOffset += LINE_HEIGHT;
                    var entryPos = new Rect(columnWidth + H_PADDING, yOffset - boxStartY, entryWidth, LINE_HEIGHT);
                    port.Draw(SimConnections, entryPos);
                }

                foreach (var reference in component.References)
                {
                    yOffset += LINE_HEIGHT;
                    var entryPos = new Rect(columnWidth + H_PADDING, yOffset - boxStartY, entryWidth, LINE_HEIGHT);
                    reference.Draw(SimConnections, entryPos);
                }

                GUI.EndGroup();

                yOffset += LINE_HEIGHT + H_PADDING;
            }

            GUI.EndScrollView();
        }

        const int H_PADDING = 10;
        const int LINE_HEIGHT = 20;

        private class ComponentInfo
        {
            public readonly SimComponentDefinitionProxy Component;
            public PortIdField[] IdFields;
            public PortDefInfo[] Ports;
            public PortReferenceInfo[] References;

            public float Height => (1 + IdFields.Length + Ports.Length + References.Length) * LINE_HEIGHT;
            public float PaddedHeight => Height + (H_PADDING * 2);

            public ComponentInfo(SimComponentDefinitionProxy component)
            {
                Component = component;
                IdFields = new PortIdField[0];
                Ports = component.ExposedPorts.Select(p => new PortDefInfo(component.ID, p)).ToArray();
                References = component.ExposedPortReferences.Select(p => new PortReferenceInfo(component.ID, p)).ToArray();
            }
        }

        

        private static string PortIdOrWarning(string? portId)
        {
            if (!string.IsNullOrWhiteSpace(portId)) return portId!;
            return "! Null Connection !";
        }

        private static bool TryFindInputConnection(SimConnectionsDefinitionProxy connections, string targetId, out ConnectionDescriptor result)
        {
            if (connections.connections.FirstOrDefault(c => c.fullPortIdIn == targetId) is PortConnectionProxy conn)
            {
                result = new ConnectionDescriptor(connections, PortIdOrWarning(conn.fullPortIdOut), conn, PortDirection.Input);
                return true;
            }
            if (connections.portReferenceConnections.FirstOrDefault(r => r.portReferenceId == targetId) is PortReferenceConnectionProxy refConn)
            {
                result = new ConnectionDescriptor(connections, PortIdOrWarning(refConn.portId), refConn, PortDirection.Input);
                return true;
            }

            result = null!;
            return false;
        }

        private static bool TryFindOutputConnection(SimConnectionsDefinitionProxy connections, string sourceId, out ConnectionDescriptor result)
        {
            if (connections.connections.FirstOrDefault(c => c.fullPortIdOut == sourceId) is PortConnectionProxy conn)
            {
                result = new ConnectionDescriptor(connections, PortIdOrWarning(conn.fullPortIdIn), conn, PortDirection.Output);
                return true;
            }
            if (connections.portReferenceConnections.FirstOrDefault(r => r.portId == sourceId) is PortReferenceConnectionProxy refConn)
            {
                result = new ConnectionDescriptor(connections, PortIdOrWarning(refConn.portReferenceId), refConn, PortDirection.Output);
                return true;
            }

            result = null!;
            return false;
        }

        private abstract class ComponentEntry
        {
            protected const string NOT_CONNECTED = "Not Connected";

            public abstract void Draw(SimConnectionsDefinitionProxy connections, Rect position);

            protected Rect GetLeftButtonRect(Rect position)
            {
                return new Rect(
                    position.x - (position.width + (H_PADDING * 2)),
                    position.y,
                    position.width,
                    LINE_HEIGHT
                );
            }

            protected Rect GetRightButtonRect(Rect position)
            {
                return new Rect(
                    position.x + position.width + (H_PADDING * 2),
                    position.y,
                    position.width,
                    LINE_HEIGHT
                );
            }
        }

        private class PortIdField : ComponentEntry
        {
            public override void Draw(SimConnectionsDefinitionProxy connections, Rect position)
            {
                throw new System.NotImplementedException();
            }
        }

        private class PortDefInfo : ComponentEntry
        {
            public readonly string FullId;
            public readonly PortDefinition Port;

            public PortDefInfo(string compId, PortDefinition port)
            {
                FullId = $"{compId}.{port.ID}";
                Port = port;
            }

            public override void Draw(SimConnectionsDefinitionProxy connections, Rect position)
            {
                GUI.Label(position, $"[Port] {Port.ID}");

                if (Port.type == DVPortType.EXTERNAL_IN)
                {
                    var lRect = GetLeftButtonRect(position);
                    GUI.Label(lRect, "[External Control]");
                }
                else if (Port.type == DVPortType.IN)
                {
                    var lRect = GetLeftButtonRect(position);

                    TryFindInputConnection(connections, FullId, out var inConnection);
                    string label = inConnection?.Id ?? NOT_CONNECTED;

                    if (GUI.Button(lRect, label))
                    {
                        var selector = new PortConnectionSelector(connections, lRect.width, Port, FullId, PortDirection.Input, inConnection);
                        PopupWindow.Show(lRect, selector);
                    }
                }
                else
                {
                    var rRect = GetRightButtonRect(position);

                    TryFindOutputConnection(connections, FullId, out var outConnection);
                    string label = outConnection?.Id ?? NOT_CONNECTED;

                    if (GUI.Button(rRect, label))
                    {
                        var selector = new PortConnectionSelector(connections, rRect.width, Port, FullId, PortDirection.Output, outConnection);
                        PopupWindow.Show(rRect, selector);
                    }
                }
            }
        }

        private class PortReferenceInfo : ComponentEntry
        {
            public readonly string FullId;
            public readonly PortReferenceDefinition Reference;

            public PortReferenceInfo(string compId, PortReferenceDefinition portRef)
            {
                FullId = $"{compId}.{portRef.ID}";
                Reference = portRef;
            }

            public override void Draw(SimConnectionsDefinitionProxy connections, Rect position)
            {
                GUI.Label(position, $"[Ref] {Reference.ID}");

                var lRect = GetLeftButtonRect(position);
                TryFindInputConnection(connections, FullId, out var inConnection);
                string label = inConnection?.Id ?? NOT_CONNECTED;

                if (GUI.Button(lRect, label))
                {
                    var popup = new PortConnectionSelector(connections, lRect.width, Reference, FullId, inConnection);
                    PopupWindow.Show(lRect, popup);
                }
            }
        }
    }
}