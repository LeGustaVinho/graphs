using System;
using System.Collections.Generic;

namespace LegendaryTools.GraphV2
{
    public class Node : INode
    {
        public string Id { get; set; }
        public List<INodeConnection> Connections { get; }

        public Node()
        {
            Id = Guid.NewGuid().ToString();
            Connections = new List<INodeConnection>();
        }

        public virtual INode[] Neighbours
        {
            get
            {
                List<INode> neighboursList = new List<INode>();
                foreach (INodeConnection conn in Connections)
                {
                    INode neighbour = null;
                    if (conn.Direction == NodeConnectionDirection.Unidirectional)
                    {
                        neighbour = conn.ToNode;
                    }
                    else
                    {
                        neighbour = conn.FromNode == this ? conn.ToNode : conn.FromNode;
                    }

                    // Adiciona o vizinho se ainda não estiver na lista
                    if (neighbour != null && !neighboursList.Contains(neighbour))
                        neighboursList.Add(neighbour);
                }
                return neighboursList.ToArray();
            }
        }

        public INodeConnection[] OutboundConnections
        {
            get
            {
                List<INodeConnection> outbound = new List<INodeConnection>();
                foreach (INodeConnection conn in Connections)
                {
                    if (conn.FromNode == this)
                        outbound.Add(conn);
                }
                return outbound.ToArray();
            }
        }

        public INodeConnection[] InboundConnections
        {
            get
            {
                List<INodeConnection> inbound = new List<INodeConnection>();
                foreach (INodeConnection conn in Connections)
                {
                    if (conn.ToNode == this)
                        inbound.Add(conn);
                }
                return inbound.ToArray();
            }
        }

        public int Count => Connections.Count;

        public INodeConnection ConnectTo(INode to, NodeConnectionDirection direction, float weight = 1.0f)
        {
            if (to == null) throw new ArgumentNullException(nameof(to));
            if (Equals(to)) throw new InvalidOperationException("Cannot connect node to itself.");

            INodeConnection existingConnection = null;

            foreach (INodeConnection conn in Connections)
            {
                bool condition1 = conn.ToNode == to && conn.FromNode == this;
                bool condition2 = conn.Direction == NodeConnectionDirection.Bidirectional &&
                                  conn.FromNode == to && conn.ToNode == this;

                if (condition1 || condition2)
                {
                    existingConnection = conn;
                    break;
                }
            }

            if (existingConnection != null)
                // Optionally update the existing connection
                return existingConnection;

            NodeConnection connection = new NodeConnection(this, to, direction, weight);
            Connections.Add(connection);
            to.Connections.Add(connection);
            return connection;
        }

        public bool RemoveConnection(INodeConnection nodeConnection)
        {
            if (Connections.Remove(nodeConnection))
            {
                if (nodeConnection.Direction == NodeConnectionDirection.Bidirectional)
                    nodeConnection.ToNode.RemoveConnection(nodeConnection);
                return true;
            }

            return false;
        }
    }
}