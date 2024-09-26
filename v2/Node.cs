using System;
using System.Collections.Generic;

namespace LegendaryTools.GraphV2
{
    public class Node : INode
    {
        public string Id { get; set; }
        public List<INodeConnection> Connections { get; }
        public IGraph Owner { get; set; }

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
                        if(conn.ToNode != this)
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
                    switch (conn.Direction)
                    {
                        case NodeConnectionDirection.Unidirectional:
                        {
                            if (conn.FromNode == this)
                                outbound.Add(conn);
                            break;
                        }
                        case NodeConnectionDirection.Bidirectional:
                        {
                            if (conn.FromNode == this || conn.ToNode == this)
                                outbound.Add(conn);
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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
                    switch (conn.Direction)
                    {
                        case NodeConnectionDirection.Unidirectional:
                        {
                            if (conn.ToNode == this)
                                inbound.Add(conn);
                            break;
                        }
                        case NodeConnectionDirection.Bidirectional:
                        {
                            if (conn.FromNode == this || conn.ToNode == this)
                                inbound.Add(conn);
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                return inbound.ToArray();
            }
        }

        public int Count => Connections.Count;

        public INodeConnection ConnectTo(INode to, NodeConnectionDirection newDirection)
        {
            if (to == null) throw new ArgumentNullException(nameof(to));
            if (Equals(to)) throw new InvalidOperationException("Cannot connect node to itself.");
            if (!Enum.IsDefined(typeof(NodeConnectionDirection), newDirection)) throw new ArgumentException($"{newDirection} is not a valid enum type for {nameof(NodeConnectionDirection)}");
            
            INode from = this;
            foreach (INodeConnection conn in Connections)
            {
                bool isSameNodes = conn.ToNode == to && conn.FromNode == from; //Same nodes
                bool isInversedNode = conn.FromNode == to && conn.ToNode == from;
                switch (newDirection)
                {
                    case NodeConnectionDirection.Unidirectional
                        when conn.Direction == NodeConnectionDirection.Unidirectional:
                    {
                        if (isSameNodes) return conn; //Is exact the same
                        if (isInversedNode)
                        {
                            //Update current connection to Bidirectional
                            conn.Direction = NodeConnectionDirection.Bidirectional;
                            return conn;
                        }
                        break;
                    }
                    case NodeConnectionDirection.Unidirectional
                        when conn.Direction == NodeConnectionDirection.Bidirectional:
                    {
                        if (isSameNodes || isInversedNode) return conn; //Redundant connection
                        break;
                    }
                    case NodeConnectionDirection.Bidirectional
                        when conn.Direction == NodeConnectionDirection.Unidirectional:
                    {
                        if (isSameNodes || isInversedNode)
                        {
                            //Update current connection to Bidirectional
                            conn.Direction = NodeConnectionDirection.Bidirectional;
                            return conn;
                        }
                        break;
                    }
                    case NodeConnectionDirection.Bidirectional
                        when conn.Direction == NodeConnectionDirection.Bidirectional:
                    {
                        if (isSameNodes || isInversedNode) return conn; //Redundant connection
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            NodeConnection connection = new NodeConnection(this, to, newDirection);
            Connections.Add(connection);
            to.Connections.Add(connection);
            return connection;
        }

        public bool RemoveConnection(INodeConnection nodeConnection)
        {
            if (Connections.Remove(nodeConnection))
            {
                nodeConnection.ToNode.RemoveConnection(nodeConnection);
                return true;
            }

            return false;
        }

        void INode.SetOwner(IGraph owner)
        {
            Owner = owner;
        }
    }
}