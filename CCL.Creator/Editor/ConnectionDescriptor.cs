using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace CCL.Creator.Editor
{
    public enum PortDirection
    {
        Any = 0,
        Input,
        Output,
    }

    public static class PortDirectionExtension
    {
        public static PortDirection Opposite(this PortDirection direction)
        {
            return direction switch
            {
                PortDirection.Input => PortDirection.Output,
                PortDirection.Output => PortDirection.Input,
                _ => PortDirection.Any
            };
        }
    }

    public class ConnectionDescriptor
    {
        public readonly SimConnectionsDefinitionProxy Connections;
        public readonly string Id;
        public readonly PortConnectionProxy? PortConnection;
        public readonly PortReferenceConnectionProxy? ReferenceConnection;
        public readonly PortIdField? IdFieldConnection;
        public readonly string? AssignedId;

        public readonly PortDirection Direction;
        public readonly PortOptionType ConnectionType;

        public ConnectionDescriptor(SimConnectionsDefinitionProxy host, string id, PortConnectionProxy portConnection, PortDirection direction)
        {
            Connections = host;
            Id = id;
            PortConnection = portConnection;
            Direction = direction;
            ConnectionType = PortOptionType.Port;
        }

        public ConnectionDescriptor(SimConnectionsDefinitionProxy host, string id, PortReferenceConnectionProxy portReferenceConnection, PortDirection direction)
        {
            Connections = host;
            Id = id;
            ReferenceConnection = portReferenceConnection;
            Direction = direction;
            ConnectionType = PortOptionType.PortReference;
        }

        public ConnectionDescriptor(SimConnectionsDefinitionProxy host, PortIdField idFieldConnection, string assignedId, PortDirection direction)
        {
            Connections = host;
            Id = (direction == PortDirection.Output) ? idFieldConnection.FullName : assignedId;
            IdFieldConnection = idFieldConnection;
            AssignedId = assignedId;
            Direction = direction;
            ConnectionType = PortOptionType.PortIdField;
        }

        public void DestroyConnection()
        {
            if (PortConnection != null)
            {
                Connections.connections.Remove(PortConnection);
            }
            else if (ReferenceConnection != null)
            {
                Connections.portReferenceConnections.Remove(ReferenceConnection);
            }
            else if (IdFieldConnection != null)
            {
                IdFieldConnection.UnAssign(AssignedId!);

                EditorUtility.SetDirty(IdFieldConnection.Container);
                SimulationEditorWindow.SaveAndRefresh();
            }
        }

        public void ChangeLink(PortOptionBase newLink)
        {
            string localId = GetLocalId();

            if (newLink is PortOption newPort)
            {
                // destroy existing ref connection if necessary
                if (ReferenceConnection != null)
                {
                    Connections.portReferenceConnections.Remove(ReferenceConnection);
                }

                var link = PortConnection;
                if (link == null)
                {
                    link = new PortConnectionProxy();
                    SetIdForDirection(link, localId, Direction);
                }
                SetIdForDirection(link, newPort.ID!, Direction.Opposite());
            }
            else if (newLink is PortReferenceOption newRef)
            {
                // destroy existing port connection if necessary
                if (PortConnection != null)
                {
                    Connections.connections.Remove(PortConnection);
                }

                var link = ReferenceConnection;
                if (link == null)
                {
                    link = new PortReferenceConnectionProxy();
                    SetIdForDirection(link, localId, Direction);
                }
                SetIdForDirection(link, newRef.ID!, Direction.Opposite());
            }
        }

        public static ConnectionDescriptor CreateNewLink(SimConnectionsDefinitionProxy host, string localId, bool localIsRef, PortOptionBase remote, PortDirection direction)
        {
            if (remote is PortIdFieldOption idField)
            {
                idField.Field.Assign(localId);
                EditorUtility.SetDirty(idField.Field.Container);
                SimulationEditorWindow.SaveAndRefresh();
                return new ConnectionDescriptor(host, idField.Field, localId, direction);
            }

            if (localIsRef || remote is PortReferenceOption)
            {
                var link = new PortReferenceConnectionProxy();
                SetIdForDirection(link, localId, direction);
                SetIdForDirection(link, remote.ID!, direction.Opposite());
                host.portReferenceConnections.Add(link);
                return new ConnectionDescriptor(host, remote.ID!, link, direction);
            }
            else
            {
                var link = new PortConnectionProxy();
                SetIdForDirection(link, localId, direction);
                SetIdForDirection(link, remote.ID!, direction.Opposite());
                host.connections.Add(link);
                return new ConnectionDescriptor(host, remote.ID!, link, direction);
            }
        }

        public static ConnectionDescriptor CreatePortIdLink(SimConnectionsDefinitionProxy host, PortIdField idField, string fullPortId)
        {
            idField.Assign(fullPortId);
            EditorUtility.SetDirty(idField.Container);
            SimulationEditorWindow.SaveAndRefresh();
            return new ConnectionDescriptor(host, idField, fullPortId, PortDirection.Input);
        }

        private string GetLocalId()
        {
            if (PortConnection != null)
            {
                return (Direction == PortDirection.Output) ? PortConnection.fullPortIdOut : PortConnection.fullPortIdIn;
            }
            else
            {
                return (Direction == PortDirection.Output) ? ReferenceConnection!.portId : ReferenceConnection!.portReferenceId;
            }
        }

        private static void SetIdForDirection(PortConnectionProxy connection, string fullId, PortDirection direction)
        {
            if (direction == PortDirection.Output)
            {
                connection.fullPortIdOut = fullId;
            }
            else
            {
                connection.fullPortIdIn = fullId;
            }
        }

        private static void SetIdForDirection(PortReferenceConnectionProxy connection, string fullId, PortDirection direction)
        {
            if (direction == PortDirection.Output)
            {
                connection.portId = fullId;
            }
            else
            {
                connection.portReferenceId = fullId;
            }
        }
    }

    public enum ConnectionResultType
    {
        None,
        Single,
        Multiple,
    }

    public class ConnectionResult
    {
        public readonly List<ConnectionDescriptor> Matches = new List<ConnectionDescriptor>();

        private const string NOT_CONNECTED = "Not Connected";
        private const string MULTI_CONNECTED = "[Multiple Connections]";

        public bool AnyConnection => Type != ConnectionResultType.None;
        public ConnectionDescriptor? Single => (Matches.Count == 1) ? Matches[0] : null;

        public ConnectionResultType Type
        {
            get
            {
                return Matches.Count switch
                {
                    0 => ConnectionResultType.None,
                    1 => ConnectionResultType.Single,
                    _ => ConnectionResultType.Multiple,
                };
            }
        }

        public string DisplayId
        {
            get
            {
                return Type switch
                {
                    ConnectionResultType.Single => Matches[0].Id,
                    ConnectionResultType.Multiple => MULTI_CONNECTED,
                    _ => NOT_CONNECTED,
                };
            }
        }

        public string? Tooltip =>
            (Type == ConnectionResultType.Multiple) ? string.Join(",", Matches.Select(m => m.Id)) : null;

        public void AddMatch(ConnectionDescriptor descriptor)
        {
            Matches.Add(descriptor);
        }

        public void DestroyAll()
        {
            foreach (var connection in Matches)
            {
                connection.DestroyConnection();
            }
        }

        public void Destroy(ConnectionDescriptor descriptor)
        {
            descriptor.DestroyConnection();
            Matches.Remove(descriptor);
        }
    }
}
