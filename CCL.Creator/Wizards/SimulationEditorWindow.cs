using CCL.Creator.Utility;
using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    public partial class SimulationEditorWindow : EditorWindow
    {
        private static SimulationEditorWindow? _instance;

        static SimulationEditorWindow()
        {
            PrefabStage.prefabStageClosing += OnStageClose;
        }

        private static void OnStageClose(PrefabStage _)
        {
            if (_instance) _instance!.Close();
        }

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
            SimEditorIcons.Reset();
        }

        public static void RefreshWindowFromScene()
        {
            SimConnectionsDefinitionProxy? connections;

            if (PrefabStageUtility.GetCurrentPrefabStage() is PrefabStage stage)
            {
                connections = stage.prefabContentsRoot.GetComponentInChildren<SimConnectionsDefinitionProxy>();
            }
            else
            {
                var scene = EditorSceneManager.GetActiveScene();
                var roots = scene.GetRootGameObjects();
                connections = roots.Select(r => r.GetComponentInChildren<SimConnectionsDefinitionProxy>()).FirstOrDefault();
            }

            if (connections) ShowWindow(connections);
        }

        public SimConnectionsDefinitionProxy SimConnections = null!;
        private Vector2 _scrollPosition = Vector2.zero;

        private readonly Dictionary<Component, bool> _unfoldedState = new Dictionary<Component, bool>();

        private enum SortMode
        {
            PORT_TYPE,
            VALUE_TYPE,
            NAME,
        }

        private static readonly string[] _sortModeNames = new[]
        {
            "Sort by Port Type",
            "Sort by Value Type",
            "Sort by ID",
        };

        private static SortMode _sortMode = SortMode.VALUE_TYPE;

        public void OnGUI()
        {
            if (!SimConnections) Close();

            var componentInfos = SimConnections.executionOrder?.Select(c => new ComponentInfo(c, _sortMode)).ToList() ?? new List<ComponentInfo>(0);
            foreach (var hasFields in SimConnections.transform.root.GetComponentsInChildren<IHasPortIdFields>())
            {
                if (!componentInfos.Any(info => info.Component == (Component)hasFields))
                {
                    componentInfos.Add(new ComponentInfo((Component)hasFields, _sortMode));
                }
            }
            foreach (var hasFields in SimConnections.transform.root.GetComponentsInChildren<IHasFuseIdFields>())
            {
                if (!componentInfos.Any(info => info.Component == (Component)hasFields))
                {
                    componentInfos.Add(new ComponentInfo((Component)hasFields, _sortMode));
                }
            }

            GUI.color = EditorHelpers.Colors.DEFAULT;
            // Sort mode selector
            Rect sortSelectArea = new Rect(0, 0, position.width / 4, LINE_HEIGHT);
            _sortMode = (SortMode)EditorGUI.Popup(sortSelectArea, (int)_sortMode, _sortModeNames);

            // Headers
            float compWidth = position.width - (H_PADDING * 4);
            Rect typeRect = new Rect(H_PADDING, LINE_HEIGHT, compWidth / 8, LINE_HEIGHT);
            GUI.Label(typeRect, "Port Type", "box");
            
            Rect valueRect = new Rect(compWidth / 8 + H_PADDING, LINE_HEIGHT, compWidth / 8, LINE_HEIGHT);
            GUI.Label(valueRect, "Value Type", "box");

            Rect idRect = new Rect(compWidth / 4 + H_PADDING, LINE_HEIGHT, compWidth / 4, LINE_HEIGHT);
            GUI.Label(idRect, "ID", "box");

            Rect connectRect = new Rect(compWidth / 2 + H_PADDING, LINE_HEIGHT, compWidth / 2, LINE_HEIGHT);
            GUI.Label(connectRect, "Connection(s)", "box");

            // Component scroll view
            Rect scrollArea = new Rect(0, LINE_HEIGHT * 2, position.width, position.height - LINE_HEIGHT * 2);
            Rect canvas = new Rect(0, 0, position.width - (H_PADDING * 2), componentInfos.Sum(c => c.PaddedHeight(_unfoldedState)));
            _scrollPosition = GUI.BeginScrollView(scrollArea, _scrollPosition, canvas);

            float compBoxWidth = canvas.width - (H_PADDING * 2.5f);

            float yOffset = 0;
            foreach (var component in componentInfos)
            {
                component.Draw(SimConnections, ref yOffset, compBoxWidth, _unfoldedState);
            }

            GUI.EndScrollView();
        }

        const int N_COLUMNS = 2;
        const int H_PADDING = 10;
        const int LINE_HEIGHT = 20;

        private class ComponentInfo
        {
            public readonly string Title;
            public readonly Component Component;
            public List<ComponentEntry> Entries = new List<ComponentEntry>();

            public float Height(Dictionary<Component, bool> unfolded)
            {
                bool isOpen = unfolded.ContainsKey(Component) && unfolded[Component];
                return isOpen ? ((1 + Entries.Count) * LINE_HEIGHT) : LINE_HEIGHT;
            }
            
            public float PaddedHeight(Dictionary<Component, bool> unfolded) => Height(unfolded) + (H_PADDING * 2);

            public ComponentInfo(Component component, SortMode sortMode)
            {
                Component = component;

                if (component is SimComponentDefinitionProxy simProxy)
                {
                    Title = $"{simProxy.ID} ({simProxy.GetType().Name.Replace("Proxy", string.Empty)})";
                    Entries.AddRange(simProxy.ExposedPorts.Select(p => new PortDefInfo(simProxy.ID, p)));
                    Entries.AddRange(simProxy.ExposedPortReferences.Select(p => new PortReferenceInfo(simProxy.ID, p)));
                    Entries.AddRange(simProxy.ExposedFuses.Select(p => new FuseDefInfo(simProxy.ID, p)));
                }
                else
                {
                    Title = $"{component.name} ({component.GetType().Name.Replace("Proxy", string.Empty)})";
                }

                if (component is IHasPortIdFields hasFields)
                {
                    Entries.AddRange(hasFields.ExposedPortIdFields.Select(f => new PortIdFieldInfo(f)));
                }

                if (component is IHasFuseIdFields hasFuseIds)
                {
                    Entries.AddRange(hasFuseIds.ExposedFuseIdFields.Select(f => new FuseIdFieldInfo(f)));
                }

                var comparer = new EntryComparer(sortMode);
                Entries.Sort(comparer);
            }

            public void Draw(SimConnectionsDefinitionProxy simConnections, ref float yOffset, float compBoxWidth, Dictionary<Component, bool> unfolded)
            {
                yOffset += H_PADDING;
                float columnWidth = compBoxWidth / N_COLUMNS;
                float entryWidth = columnWidth - (H_PADDING * 2);

                var compBox = new Rect(H_PADDING, yOffset, compBoxWidth, Height(unfolded));
                GUI.BeginGroup(compBox, string.Empty, "box");

                yOffset -= LINE_HEIGHT / 2f;
                float localY = 0;

                bool newFold = false;
                if (Entries.Count > 0)
                {
                    // Fold arrow
                    var foldBox = new Rect(H_PADDING, localY, compBoxWidth, LINE_HEIGHT);
                    bool currentFold = unfolded.ContainsKey(Component) && unfolded[Component];
                    newFold = EditorGUI.Foldout(foldBox, currentFold, "Expand");
                    unfolded[Component] = newFold;
                }
                // Title/Jump button
                var titleBox = new Rect(columnWidth / 2, localY, columnWidth, LINE_HEIGHT);
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
                    foreach (var entry in Entries)
                    {
                        localY += LINE_HEIGHT;
                        var entryPos = new Rect(H_PADDING, localY, entryWidth, LINE_HEIGHT);
                        entry.Draw(simConnections, entryPos);
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
                if (field.IsIdAssigned(targetId))
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
                if (field.IsIdAssigned(sourceId))
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
                    if (idField.IsIdAssigned(fullId))
                    {
                        result.AddMatch(new ConnectionDescriptor(connections, idField, fullId, PortDirection.Input));
                    }
                }
            }

            return result;
        }

        private static ConnectionResult TryFindFuseConnection(SimConnectionsDefinitionProxy connections, string fuseId)
        {
            var result = new ConnectionResult();
            var sources = PortOptionHelper.GetAvailableSources(connections, false);
            var assignedFields = PortOptionHelper.GetFuseIdFields(sources).Where(f => f.IsAssigned);

            foreach (var field in assignedFields)
            {
                if (field.IsIdAssigned(fuseId))
                {
                    result.AddMatch(new ConnectionDescriptor(connections, field, fuseId, PortDirection.Output));
                }
            }
            return result;
        }

        private static ConnectionResult TryFindFuseIdConnection(SimConnectionsDefinitionProxy connections, FuseIdField idField)
        {
            var result = new ConnectionResult();

            foreach (var component in connections.executionOrder)
            {
                foreach (var fuse in component.ExposedFuses)
                {
                    string fullId = $"{component.ID}.{fuse.id}";
                    if (idField.IsIdAssigned(fullId))
                    {
                        result.AddMatch(new ConnectionDescriptor(connections, idField, fullId, PortDirection.Input));
                    }
                }
            }

            return result;
        }
    }

    public class SimEditorIcons
    {
        private static readonly Dictionary<string, Texture2D?> _icons = new Dictionary<string, Texture2D?>();

        public static void Reset()
        {
            _icons.Clear();
        }

        private static bool TryGetValidTexture(string name, out Texture2D texture)
        {
            if (_icons.TryGetValue(name, out texture!))
            {
                return texture;
            }
            else
            {
                _icons[name] = LoadTexture(name);
            }

            return false;
        }

        public static bool TryGetPortType(string portType, out Texture2D icon) => TryGetValidTexture($"T_{portType}", out icon);

        public static bool TryGetValueType(string valueType, out Texture2D icon) => TryGetValidTexture($"V_{valueType}", out icon);

        private static Texture2D? LoadTexture(string name)
        {
            string assetPath = $"Assets/CarCreator/Icons/{name}.png";

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture)
            {
                _icons.Add(name, texture);
                return texture;
            }
            return null;
        }
    }
}