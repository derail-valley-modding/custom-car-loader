using CCL.Types;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CCL.Creator.Editor
{
    public class SimulationEditorWindow : EditorWindow
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

                // Fold arrow
                var foldBox = new Rect(H_PADDING, localY, compBoxWidth, LINE_HEIGHT);
                bool currentFold = unfolded.ContainsKey(Component) && unfolded[Component];
                bool newFold = EditorGUI.Foldout(foldBox, currentFold, "Expand");
                unfolded[Component] = newFold;

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

        private abstract class ComponentEntry
        {
            protected static readonly Color NC_COLOR = new Color32(255, 230, 128, 255);

            public readonly string DisplayID;
            public readonly string PortTypeID;
            public readonly string PortValueID;

            public ComponentEntry(string displayId, string portTypeID, string portValueID)
            {
                DisplayID = displayId;
                PortTypeID = portTypeID;
                PortValueID = portValueID;
            }

            public abstract void Draw(SimConnectionsDefinitionProxy connections, Rect position);

            protected static int CompareEnumNames<TEnum>(TEnum a, TEnum b) where TEnum : Enum
            {
                return Enum.GetName(typeof(TEnum), a).CompareTo(Enum.GetName(typeof(TEnum), b));
            }

            protected static string GetPortTypeString(DVPortType portType)
            {
                return Enum.GetName(typeof(DVPortType), portType);
            }

            protected static string GetValueTypeString(DVPortValueType valueType)
            {
                return Enum.GetName(typeof(DVPortValueType), valueType);
            }

            protected static string GetValueTypeString(DVPortValueType[]? valueFilters)
            {
                if (valueFilters == null)
                {
                    return GetValueTypeString(DVPortValueType.GENERIC);
                }
                else
                {
                    return (valueFilters.Length == 1) ? GetValueTypeString(valueFilters[0]) : "MULTI";
                }
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

            private const float DESC_COLUMNS = 4;

            private static Rect GetPortTypeRect(Rect position)
            {
                return new Rect(position.x, position.y, position.width / DESC_COLUMNS, LINE_HEIGHT);
            }

            private static Rect GetTypeRect(Rect position)
            {
                return new Rect(position.x + (position.width / DESC_COLUMNS), position.y, position.width / DESC_COLUMNS, LINE_HEIGHT);
            }

            private static Rect GetIdRect(Rect position)
            {
                const float ID_PROPORTION = (DESC_COLUMNS - 2) / DESC_COLUMNS;
                return new Rect(position.x + (position.width / DESC_COLUMNS) * 2, position.y, position.width * ID_PROPORTION, LINE_HEIGHT);
            }

            private static readonly Regex _titleRegex = new Regex(@"(?<1>\p{Lu})(?=\p{Ll})|(?<=\p{Ll})(?<1>\p{Lu})");

            private static string GetPrettyId(string id)
            {
                if (id.All(c => c == '_' || char.IsUpper(c)))
                {
                    char[] dest = new char[id.Length];

                    char lastChar = '\0';
                    for (int i = 0; i < id.Length; i++)
                    {
                        if (lastChar == '\0' || lastChar == '_')
                        {
                            dest[i] = id[i];
                        }
                        else if (id[i] == '_')
                        {
                            dest[i] = ' ';
                        }
                        else
                        {
                            dest[i] = char.ToLower(id[i]);
                        }

                        lastChar = id[i];
                    }

                    return new string(dest);
                }
                else
                {
                    var spaced = _titleRegex.Replace(id, " $1").ToCharArray();
                    spaced[0] = char.ToUpper(spaced[0]);
                    return new string(spaced);
                }
            }

            protected void DrawDescription(Rect position)
            {
                GUI.color = Color.white;

                SimEditorIcons.TryGetPortType(PortTypeID, out Texture2D typeIcon);
                GUIContent typeLabel = new GUIContent(GetPrettyId(PortTypeID), typeIcon);
                GUI.Label(GetPortTypeRect(position), typeLabel);

                SimEditorIcons.TryGetValueType(PortValueID, out Texture2D icon);
                GUIContent valueLabel = new GUIContent(GetPrettyId(PortValueID), icon);

                GUI.Label(GetTypeRect(position), valueLabel);
                GUI.Label(GetIdRect(position), GetPrettyId(DisplayID));
            }
        }

        private class EntryComparer : IComparer<ComponentEntry>
        {
            private static int CompareType(ComponentEntry x, ComponentEntry y) => x.PortTypeID.CompareTo(y.PortTypeID);
            private static int CompareValue(ComponentEntry x, ComponentEntry y) => x.PortValueID.CompareTo(y.PortValueID);
            private static int CompareName(ComponentEntry x, ComponentEntry y) => x.DisplayID.CompareTo(y.DisplayID);

            private readonly Comparison<ComponentEntry>[] _sortOrder;

            public EntryComparer(SortMode sortMode)
            {
                _sortOrder = sortMode switch
                {
                    SortMode.PORT_TYPE => new Comparison<ComponentEntry>[] { CompareType, CompareValue, CompareName },
                    SortMode.NAME => new Comparison<ComponentEntry>[] { CompareName, CompareType, CompareValue },
                    _ => new Comparison<ComponentEntry>[] { CompareValue, CompareType, CompareName },
                };
            }

            public int Compare(ComponentEntry x, ComponentEntry y)
            {
                int result;

                foreach (var comparison in _sortOrder)
                {
                    result = comparison(x, y);
                    if (result != 0) return result;
                }

                return 0;
            }
        }

        private class PortDefInfo : ComponentEntry
        {
            public readonly string FullId;
            public readonly PortDefinition Port;

            public PortDefInfo(string compId, PortDefinition port)
                : base(port.ID, GetPortTypeString(port.type), GetValueTypeString(port.valueType))
            {
                FullId = $"{compId}.{port.ID}";
                Port = port;
            }

            public override void Draw(SimConnectionsDefinitionProxy connections, Rect position)
            {
                DrawDescription(position);

                var selectorRect = GetRightButtonRect(position);

                if (Port.type == DVPortType.IN)
                {
                    var inConnection = TryFindInputConnection(connections, FullId);
                    GUI.color = inConnection.AnyConnection ? Color.white : NC_COLOR;

                    if (GUI.Button(selectorRect, new GUIContent(inConnection.DisplayId, inConnection.Tooltip)))
                    {
                        var selector = new PortConnectionSelector(connections, selectorRect.width, Port, FullId, PortDirection.Input, inConnection);
                        PopupWindow.Show(selectorRect, selector);
                    }
                }
                else
                {
                    var outConnection = TryFindOutputConnection(connections, FullId);
                    GUI.color = (outConnection.AnyConnection) ? Color.white : NC_COLOR;

                    if (GUI.Button(selectorRect, new GUIContent(outConnection.DisplayId, outConnection.Tooltip)))
                    {
                        var selector = new PortConnectionSelector(connections, selectorRect.width, Port, FullId, PortDirection.Output, outConnection);
                        PopupWindow.Show(selectorRect, selector);
                    }
                }
            }
        }

        private class PortReferenceInfo : ComponentEntry
        {
            public readonly string FullId;
            public readonly PortReferenceDefinition Reference;

            public PortReferenceInfo(string compId, PortReferenceDefinition portRef)
                : base(portRef.ID, "REFERENCE", GetValueTypeString(portRef.valueType))
            {
                FullId = $"{compId}.{portRef.ID}";
                Reference = portRef;
            }

            public override void Draw(SimConnectionsDefinitionProxy connections, Rect position)
            {
                DrawDescription(position);

                var selectorRect = GetRightButtonRect(position);
                var inConnection = TryFindInputConnection(connections, FullId);
                GUI.color = inConnection.AnyConnection ? Color.white : NC_COLOR;

                if (GUI.Button(selectorRect, new GUIContent(inConnection.DisplayId, inConnection.Tooltip)))
                {
                    var popup = new PortConnectionSelector(connections, selectorRect.width, Reference, FullId, inConnection);
                    PopupWindow.Show(selectorRect, popup);
                }
            }
        }

        private class PortIdFieldInfo : ComponentEntry
        {
            public readonly PortIdField Field;

            public PortIdFieldInfo(PortIdField field)
                : base(field.FieldName, "PORT_ID", GetValueTypeString(field.ValueFilters))
            {
                Field = field;
            }

            public override void Draw(SimConnectionsDefinitionProxy connections, Rect position)
            {
                DrawDescription(position);
                
                var selectorRect = GetRightButtonRect(position);
                var connection = TryFindPortFieldConnection(connections, Field);

                GUI.color = connection.AnyConnection ? Color.white : NC_COLOR;
                if (GUI.Button(selectorRect, new GUIContent(connection.DisplayId, connection.Tooltip)))
                {
                    var popup = new PortConnectionSelector(connections, selectorRect.width, Field, connection);
                    PopupWindow.Show(selectorRect, popup);
                }
            }
        }

        private class FuseDefInfo : ComponentEntry
        {
            public readonly string FullId;
            public readonly FuseDefinition Fuse;

            public FuseDefInfo(string compId, FuseDefinition fuse)
                : base(fuse.id, "FUSE", "FUSE")
            {
                FullId = $"{compId}.{fuse.id}";
                Fuse = fuse;
            }

            public override void Draw(SimConnectionsDefinitionProxy connections, Rect position)
            {
                DrawDescription(position);

                var selectorRect = GetRightButtonRect(position);
                var connection = TryFindFuseConnection(connections, FullId);

                GUI.color = connection.AnyConnection ? Color.white : NC_COLOR;

                if (GUI.Button(selectorRect, new GUIContent(connection.DisplayId, connection.Tooltip)))
                {
                    var selector = new PortConnectionSelector(connections, selectorRect.width, Fuse, FullId, connection);
                    PopupWindow.Show(selectorRect, selector);
                }
            }
        }

        private class FuseIdFieldInfo : ComponentEntry
        {
            public FuseIdField Field;

            public FuseIdFieldInfo(FuseIdField field)
                : base(field.FieldName, "FUSE_ID", "FUSE")
            {
                Field = field;
            }

            public override void Draw(SimConnectionsDefinitionProxy connections, Rect position)
            {
                DrawDescription(position);

                var selectorRect = GetRightButtonRect(position);
                var connection = TryFindFuseIdConnection(connections, Field);

                GUI.color = connection.AnyConnection ? Color.white : NC_COLOR;
                if (GUI.Button(selectorRect, new GUIContent(connection.DisplayId, connection.Tooltip)))
                {
                    var popup = new PortConnectionSelector(connections, selectorRect.width, Field, connection);
                    PopupWindow.Show(selectorRect, popup);
                }
            }
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