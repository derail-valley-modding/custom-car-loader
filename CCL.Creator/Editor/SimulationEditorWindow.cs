using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
            if (!SimConnections) Close();

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
                GUI.BeginGroup(compBox, string.Empty, "box");
                
                yOffset -= LINE_HEIGHT / 2f;
                float boxStartY = yOffset;

                var titleBox = new Rect(columnWidth, yOffset - boxStartY, columnWidth, LINE_HEIGHT);
                string title = $"{component.Component.ID} ({component.Component.GetType().Name.Replace("Proxy", string.Empty)})";
                if (GUI.Button(titleBox, title))
                {
                    Selection.activeObject = component.Component;
                }

                using (new GUIColorScope())
                {
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
            public PortDefInfo[] Ports;
            public PortReferenceInfo[] References;

            public float Height => (1 + Ports.Length + References.Length) * LINE_HEIGHT;
            public float PaddedHeight => Height + (H_PADDING * 2);

            public ComponentInfo(SimComponentDefinitionProxy component)
            {
                Component = component;
                Ports = component.ExposedPorts.Select(p => new PortDefInfo(component.ID, p)).ToArray();
                References = component.ExposedPortReferences.Select(p => new PortReferenceInfo(component.ID, p)).ToArray();
            }
        }

        private static string PortIdOrWarning(string? portId)
        {
            if (!string.IsNullOrWhiteSpace(portId)) return portId!;
            return "! Null Connection !";
        }

        private static ConnectionResult TryFindInputConnection(SimConnectionsDefinitionProxy connections, string targetId)
        {
            var result = new ConnectionResult();
            
            foreach (var conn in connections.connections.Where(c => c.fullPortIdIn == targetId))
            {
                result.AddMatch(new ConnectionDescriptor(connections, PortIdOrWarning(conn.fullPortIdOut), conn, PortDirection.Input));
            }
            foreach (var refConn in connections.portReferenceConnections.Where(r => r.portReferenceId == targetId))
            {
                result.AddMatch(new ConnectionDescriptor(connections, PortIdOrWarning(refConn.portId), refConn, PortDirection.Input));
            }

            var sources = PortOptionHelper.GetAvailableSources(connections, false);
            var assignedFields = PortOptionHelper.GetPortIdFields(sources).Where(f => f.IsAssigned);
            foreach (var field in assignedFields)
            {
                if (field.AssignedPort == targetId)
                {
                    result.AddMatch(new ConnectionDescriptor(connections, field));
                }
            }

            return result;
        }

        private static ConnectionResult TryFindOutputConnection(SimConnectionsDefinitionProxy connections, string sourceId)
        {
            var result = new ConnectionResult();

            foreach (var conn in connections.connections.Where(c => c.fullPortIdOut == sourceId))
            {
                result.AddMatch(new ConnectionDescriptor(connections, PortIdOrWarning(conn.fullPortIdIn), conn, PortDirection.Output));
            }
            foreach (var refConn in connections.portReferenceConnections.Where(r => r.portId == sourceId))
            {
                result.AddMatch(new ConnectionDescriptor(connections, PortIdOrWarning(refConn.portReferenceId), refConn, PortDirection.Output));
            }

            var sources = PortOptionHelper.GetAvailableSources(connections, false);
            var assignedFields = PortOptionHelper.GetPortIdFields(sources).Where(f => f.IsAssigned);
            foreach (var field in assignedFields)
            {
                if (field.AssignedPort == sourceId)
                {
                    result.AddMatch(new ConnectionDescriptor(connections, field));
                }
            }

            return result;
        }

        private abstract class ComponentEntry
        {
            protected static readonly Color NC_COLOR = new Color32(255, 230, 128, 255);

            public abstract void Draw(SimConnectionsDefinitionProxy connections, Rect position);

            protected static string GetValueTypeString(DVPortValueType valueType)
            {
                return System.Enum.GetName(typeof(DVPortValueType), valueType);
            }

            protected static Rect GetLeftButtonRect(Rect position)
            {
                return new Rect(
                    position.x - (position.width + (H_PADDING * 2)),
                    position.y,
                    position.width,
                    LINE_HEIGHT
                );
            }

            protected static Rect GetRightButtonRect(Rect position)
            {
                return new Rect(
                    position.x + position.width + (H_PADDING * 2),
                    position.y,
                    position.width,
                    LINE_HEIGHT
                );
            }

            private const float TYPE_RATIO = 4;

            private static Rect GetTypeRect(Rect position)
            {
                return new Rect(position.x, position.y, position.width / TYPE_RATIO, LINE_HEIGHT);
            }

            private static Rect GetIdRect(Rect position)
            {
                const float ID_PROPORTION = (TYPE_RATIO - 1) / TYPE_RATIO;
                return new Rect(position.x + (position.width / TYPE_RATIO), position.y, position.width * ID_PROPORTION, LINE_HEIGHT);
            }

            protected static void DrawDescription(Rect position, DVPortValueType valueType, string id)
            {
                GUI.color = Color.white;
                GUI.Label(GetTypeRect(position), $"[{GetValueTypeString(valueType)}]");
                GUI.Label(GetIdRect(position), id);
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
                DrawDescription(position, Port.valueType, Port.ID);

                if (Port.type == DVPortType.IN)
                {
                    var lRect = GetLeftButtonRect(position);

                    var inConnection = TryFindInputConnection(connections, FullId);
                    GUI.color = inConnection.AnyConnection ? Color.white : NC_COLOR;

                    if (GUI.Button(lRect, new GUIContent(inConnection.DisplayId, inConnection.Tooltip)))
                    {
                        var selector = new PortConnectionSelector(connections, lRect.width, Port, FullId, PortDirection.Input, inConnection);
                        PopupWindow.Show(lRect, selector);
                    }
                }
                else
                {
                    if (Port.type == DVPortType.EXTERNAL_IN)
                    {
                        GUI.color = Color.white;
                        var lRect = GetLeftButtonRect(position);
                        GUI.Label(lRect, "[External Control]");
                    }

                    var rRect = GetRightButtonRect(position);

                    var outConnection = TryFindOutputConnection(connections, FullId);
                    GUI.color = (outConnection.AnyConnection || (Port.type == DVPortType.EXTERNAL_IN)) ? Color.white : NC_COLOR;

                    if (GUI.Button(rRect, new GUIContent(outConnection.DisplayId, outConnection.Tooltip)))
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
                DrawDescription(position, Reference.valueType, Reference.ID);

                var lRect = GetLeftButtonRect(position);
                var inConnection = TryFindInputConnection(connections, FullId);
                GUI.color = inConnection.AnyConnection ? Color.white : NC_COLOR;

                if (GUI.Button(lRect, new GUIContent(inConnection.DisplayId, inConnection.Tooltip)))
                {
                    var popup = new PortConnectionSelector(connections, lRect.width, Reference, FullId, inConnection);
                    PopupWindow.Show(lRect, popup);
                }
            }
        }
    }
}