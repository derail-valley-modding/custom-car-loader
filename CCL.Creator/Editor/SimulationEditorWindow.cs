using CCL.Types;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
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

        public void OnDestroy()
        {
            _unfoldedState.Clear();
        }

        private static string? GetSelectionPath()
        {
            if (Selection.activeGameObject)
            {
                return Selection.activeGameObject.GetPath();
            }
            return null;
        }

        public static void SaveAndRefresh()
        {
            AssetDatabase.SaveAssets();
            string? selectionPath = GetSelectionPath();
            EditorApplication.delayCall += () => DelayedRefresh(selectionPath);
        }

        private static void DelayedRefresh(string? selectionPath)
        {
            GameObject? root = null;
            SimConnectionsDefinitionProxy? connections;
            var parts = selectionPath?.Split(new[] { '/' }, 2);

            if (PrefabStageUtility.GetCurrentPrefabStage() is PrefabStage stage)
            {
                root = stage.prefabContentsRoot;
                connections = root.GetComponentInChildren<SimConnectionsDefinitionProxy>();
            }
            else
            {
                var scene = EditorSceneManager.GetActiveScene();
                var roots = scene.GetRootGameObjects();
                connections = roots.Select(r => r.GetComponentInChildren<SimConnectionsDefinitionProxy>()).FirstOrDefault();

                if (parts != null)
                {
                    root = roots.FirstOrDefault(r => r.name == parts[0]);
                }
            }

            if (connections) ShowWindow(connections);

            if (parts != null && root)
            {
                if (parts.Length == 1)
                {
                    Selection.activeGameObject = root;
                }
                else
                {
                    Transform? target = root!.transform.FindSafe(parts[1]);
                    if (target)
                    {
                        Selection.activeGameObject = target!.gameObject;
                    }
                }
            }
        }

        public SimConnectionsDefinitionProxy SimConnections = null!;
        private Vector2 _scrollPosition = Vector2.zero;

        private readonly Dictionary<Component, bool> _unfoldedState = new Dictionary<Component, bool>();

        public void OnGUI()
        {
            if (!SimConnections) Close();

            var componentInfos = SimConnections.executionOrder?.Select(c => new ComponentInfo(c)).ToList() ?? new List<ComponentInfo>(0);
            foreach (var hasFields in SimConnections.transform.root.GetComponentsInChildren<IHasPortIdFields>())
            {
                if (!componentInfos.Any(info => info.Component == (Component)hasFields))
                {
                    componentInfos.Add(new ComponentInfo((Component)hasFields));
                }
            }

            Rect canvas = new Rect(0, 0, position.width - (H_PADDING * 2), componentInfos.Sum(c => c.PaddedHeight(_unfoldedState)));
            _scrollPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), _scrollPosition, canvas);

            float compBoxWidth = canvas.width - (H_PADDING * 2.5f);

            float yOffset = 0;
            foreach (var component in componentInfos)
            {
                component.Draw(SimConnections, ref yOffset, compBoxWidth, _unfoldedState);
            }

            GUI.EndScrollView();
        }

        const int H_PADDING = 10;
        const int LINE_HEIGHT = 20;

        private class ComponentInfo
        {
            public readonly string Title;
            public readonly Component Component;
            public PortDefInfo[] Ports;
            public PortReferenceInfo[] References;
            public PortIdFieldInfo[] IdFields;

            public float Height(Dictionary<Component, bool> unfolded)
            {
                bool isOpen = unfolded.ContainsKey(Component) && unfolded[Component];
                return isOpen ? ((1 + Ports.Length + References.Length + IdFields.Length) * LINE_HEIGHT) : LINE_HEIGHT;
            }
            
            public float PaddedHeight(Dictionary<Component, bool> unfolded) => Height(unfolded) + (H_PADDING * 2);

            public ComponentInfo(Component component)
            {
                Component = component;

                if (component is SimComponentDefinitionProxy simProxy)
                {
                    Title = $"{simProxy.ID} ({simProxy.GetType().Name.Replace("Proxy", string.Empty)})";
                    Ports = simProxy.ExposedPorts.Select(p => new PortDefInfo(simProxy.ID, p)).ToArray();
                    References = simProxy.ExposedPortReferences.Select(p => new PortReferenceInfo(simProxy.ID, p)).ToArray();
                }
                else
                {
                    Title = $"{component.name} ({component.GetType().Name.Replace("Proxy", string.Empty)})";
                    Ports = Array.Empty<PortDefInfo>();
                    References = Array.Empty<PortReferenceInfo>();
                }

                if (component is IHasPortIdFields hasFields)
                {
                    IdFields = hasFields.ExposedPortIdFields.Select(f => new PortIdFieldInfo(f)).ToArray();
                }
                else
                {
                    IdFields = Array.Empty<PortIdFieldInfo>();
                }
            }

            public void Draw(SimConnectionsDefinitionProxy simConnections, ref float yOffset, float compBoxWidth, Dictionary<Component, bool> unfolded)
            {
                yOffset += H_PADDING;
                float columnWidth = compBoxWidth / 3;
                float entryWidth = columnWidth - (H_PADDING * 2);

                var compBox = new Rect(H_PADDING, yOffset, compBoxWidth, Height(unfolded));
                GUI.BeginGroup(compBox, string.Empty, "box");

                yOffset -= LINE_HEIGHT / 2f;
                float localY = 0;

                // Fold arrow
                var foldBox = new Rect(H_PADDING, localY, compBoxWidth, LINE_HEIGHT);
                bool currentFold = unfolded.ContainsKey(Component) && unfolded[Component];
                bool newFold = EditorGUI.Foldout(foldBox, currentFold, "Expand");
                unfolded[Component] = newFold;

                // Title/Jump button
                var titleBox = new Rect(columnWidth, localY, columnWidth, LINE_HEIGHT);
                if (GUI.Button(titleBox, Title))
                {
                    Selection.activeObject = Component;
                }

                if (!newFold)
                {
                    GUI.EndGroup();
                    yOffset += LINE_HEIGHT + H_PADDING;
                    return;
                }

                using (new GUIColorScope())
                {
                    // Ports
                    foreach (var port in Ports)
                    {
                        localY += LINE_HEIGHT;
                        var entryPos = new Rect(columnWidth + H_PADDING, localY, entryWidth, LINE_HEIGHT);
                        port.Draw(simConnections, entryPos);
                    }

                    // Port References
                    foreach (var reference in References)
                    {
                        localY += LINE_HEIGHT;
                        var entryPos = new Rect(columnWidth + H_PADDING, localY, entryWidth, LINE_HEIGHT);
                        reference.Draw(simConnections, entryPos);
                    }

                    // Port ID Fields
                    foreach (var idField in IdFields)
                    {
                        localY += LINE_HEIGHT;
                        var entryPos = new Rect(columnWidth + H_PADDING, localY, entryWidth, LINE_HEIGHT);
                        idField.Draw(simConnections, entryPos);
                    }
                }

                GUI.EndGroup();
                yOffset += localY;
                yOffset += LINE_HEIGHT + H_PADDING;
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
                if (field.IsPortAssigned(targetId))
                {
                    result.AddMatch(new ConnectionDescriptor(connections, field, targetId, PortDirection.Output));
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
                if (field.IsPortAssigned(sourceId))
                {
                    result.AddMatch(new ConnectionDescriptor(connections, field, sourceId, PortDirection.Output));
                }
            }

            return result;
        }

        private static ConnectionResult TryFindPortFieldConnection(SimConnectionsDefinitionProxy connections, PortIdField idField)
        {
            var result = new ConnectionResult();

            foreach (var component in connections.executionOrder)
            {
                foreach (var port in component.ExposedPorts)
                {
                    string fullId = $"{component.ID}.{port.ID}";
                    if (idField.AssignedPorts.Contains(fullId))
                    {
                        result.AddMatch(new ConnectionDescriptor(connections, idField, fullId, PortDirection.Input));
                    }
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

            protected static void DrawDescription(Rect position, DVPortValueType[]? valueFilters, string id)
            {
                GUI.color = Color.white;

                string typeString;
                if (valueFilters == null)
                {
                    typeString = GetValueTypeString(DVPortValueType.GENERIC);
                }
                else
                {
                    typeString = (valueFilters.Length == 1) ? GetValueTypeString(valueFilters[0]) : "Multi";
                }

                GUI.Label(GetTypeRect(position), $"[{typeString}]");
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

        private class PortIdFieldInfo : ComponentEntry
        {
            public readonly PortIdField Field;

            public PortIdFieldInfo(PortIdField field)
            {
                Field = field;
            }

            public override void Draw(SimConnectionsDefinitionProxy connections, Rect position)
            {
                DrawDescription(position, Field.ValueFilters, Field.FieldName);
                
                var lRect = GetLeftButtonRect(position);
                var connection = TryFindPortFieldConnection(connections, Field);

                GUI.color = connection.AnyConnection ? Color.white : NC_COLOR;
                if (GUI.Button(lRect, new GUIContent(connection.DisplayId, connection.Tooltip)))
                {
                    var popup = new PortConnectionSelector(connections, lRect.width, Field, connection);
                    PopupWindow.Show(lRect, popup);
                }
            }
        }
    }
}