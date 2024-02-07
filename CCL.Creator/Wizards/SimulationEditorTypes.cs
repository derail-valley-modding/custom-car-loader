using CCL.Creator.Utility;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    public partial class SimulationEditorWindow
    {
        private abstract class ComponentEntry
        {
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
                if (id.All(c => c == '_' || char.IsDigit(c) || char.IsUpper(c)))
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
                GUI.color = EditorHelpers.Colors.DEFAULT;

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
                    GUI.color = inConnection.AnyConnection ? EditorHelpers.Colors.DEFAULT : EditorHelpers.Colors.WARNING;

                    if (GUI.Button(selectorRect, new GUIContent(inConnection.DisplayId, inConnection.Tooltip)))
                    {
                        var selector = new PortConnectionSelector(connections, selectorRect.width, Port, FullId, PortDirection.Input, inConnection);
                        PopupWindow.Show(selectorRect, selector);
                    }
                }
                else
                {
                    var outConnection = TryFindOutputConnection(connections, FullId);
                    GUI.color = (outConnection.AnyConnection) ? EditorHelpers.Colors.DEFAULT : EditorHelpers.Colors.WARNING;

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
                GUI.color = inConnection.AnyConnection ? EditorHelpers.Colors.DEFAULT : EditorHelpers.Colors.WARNING;

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

                GUI.color = connection.AnyConnection ? EditorHelpers.Colors.DEFAULT : EditorHelpers.Colors.WARNING;
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

                GUI.color = connection.AnyConnection ? EditorHelpers.Colors.DEFAULT : EditorHelpers.Colors.WARNING;

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

                GUI.color = connection.AnyConnection ? EditorHelpers.Colors.DEFAULT : EditorHelpers.Colors.WARNING;
                if (GUI.Button(selectorRect, new GUIContent(connection.DisplayId, connection.Tooltip)))
                {
                    var popup = new PortConnectionSelector(connections, selectorRect.width, Field, connection);
                    PopupWindow.Show(selectorRect, popup);
                }
            }
        }
    }
}
