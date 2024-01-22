using CCL.Types.Proxies.Ports;

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
        public readonly PortDirection Direction;

        public ConnectionDescriptor(SimConnectionsDefinitionProxy host, string id, PortConnectionProxy portConnection, PortDirection direction)
        {
            Connections = host;
            Id = id;
            PortConnection = portConnection;
            Direction = direction;
        }

        public ConnectionDescriptor(SimConnectionsDefinitionProxy host, string id, PortReferenceConnectionProxy portReferenceConnection, PortDirection direction)
        {
            Connections = host;
            Id = id;
            ReferenceConnection = portReferenceConnection;
            Direction = direction;
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

        public static void CreateNewLink(SimConnectionsDefinitionProxy host, string localId, string remoteId, PortDirection direction, bool refConnection)
        {
            if (refConnection)
            {
                var link = new PortReferenceConnectionProxy();
                SetIdForDirection(link, localId, direction);
                SetIdForDirection(link, remoteId, direction.Opposite());
                host.portReferenceConnections.Add(link);
            }
            else
            {
                var link = new PortConnectionProxy();
                SetIdForDirection(link, localId, direction);
                SetIdForDirection(link, remoteId, direction.Opposite());
                host.connections.Add(link);
            }
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
}
