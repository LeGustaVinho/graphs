using System;
using System.Collections.Generic;
using System.Linq;

namespace LegendaryTools.GraphV2
{
    public class Graph : IGraph
    {
        public string Id { get; set; }
        public IGraph ParentGraph { get; set; }
        public IGraph ChildGraph { get; set; }
        private readonly List<INode> nodes;

        public Graph()
        {
            Id = Guid.NewGuid().ToString();
            nodes = new List<INode>();
        }

        public bool IsDirectedAcyclic => IsDirected && !HasDirectedCycle();
        public bool IsDirectedCyclic => IsDirected && HasDirectedCycle();
        public bool IsAcyclic => !HasCycle();
        public bool IsCyclic => HasCycle();

        private bool IsDirected
        {
            get
            {
                return nodes.Any(node =>
                    node.Connections.Any(conn => conn.Direction == NodeConnectionDirection.Unidirectional));
            }
        }

        public IGraph[] GraphHierarchy
        {
            get
            {
                List<IGraph> hierarchy = new List<IGraph>();
                IGraph current = ParentGraph;
                while (current != null)
                {
                    hierarchy.Insert(0, current);
                    current = current.ParentGraph;
                }

                return hierarchy.ToArray();
            }
        }

        public INode[] AllNodes => nodes.ToArray();

        public INode[] AllNodesRecursive
        {
            get
            {
                List<INode> allNodes = new List<INode>(nodes);
                IGraph child = ChildGraph;
                while (child != null)
                {
                    allNodes.AddRange(child.AllNodes);
                    child = child.ChildGraph;
                }

                return allNodes.ToArray();
            }
        }

        public void Add(INode newNode)
        {
            if (newNode == null) throw new ArgumentNullException(nameof(newNode));
            if (!nodes.Contains(newNode)) nodes.Add(newNode);
        }

        public virtual bool Remove(INode node)
        {
            if (nodes.Remove(node))
            {
                // Remove all connections related to this node
                foreach (INodeConnection conn in node.Connections.ToList()) conn.Disconnect();
                return true;
            }

            return false;
        }

        public bool Contains(INode node)
        {
            return nodes.Contains(node);
        }

        public INode[] Neighbours(INode node)
        {
            if (!nodes.Contains(node)) throw new ArgumentException("Node does not exist in the graph.");
            return node.Neighbours;
        }

        private bool HasCycle()
        {
            HashSet<INode> visited = new HashSet<INode>();
            HashSet<INode> recursionStack = new HashSet<INode>();

            foreach (INode node in nodes)
                if (DetectCycle(node, visited, recursionStack, false))
                    return true;
            return false;
        }

        private bool HasDirectedCycle()
        {
            HashSet<INode> visited = new HashSet<INode>();
            HashSet<INode> recursionStack = new HashSet<INode>();

            foreach (INode node in nodes)
                if (DetectCycle(node, visited, recursionStack, true))
                    return true;
            return false;
        }

        private bool DetectCycle(INode node, HashSet<INode> visited, HashSet<INode> recursionStack, bool checkDirection)
        {
            if (!visited.Contains(node))
            {
                visited.Add(node);
                recursionStack.Add(node);

                foreach (INode neighbor in node.Neighbours)
                {
                    IEnumerable<INodeConnection> connections = node.Connections.Where(conn => conn.ToNode == neighbor ||
                        (conn.Direction == NodeConnectionDirection.Bidirectional && conn.FromNode == neighbor));

                    foreach (INodeConnection conn in connections)
                    {
                        if (checkDirection && conn.Direction != NodeConnectionDirection.Unidirectional)
                            continue;

                        if (!visited.Contains(neighbor) &&
                            DetectCycle(neighbor, visited, recursionStack, checkDirection))
                            return true;
                        if (recursionStack.Contains(neighbor))
                            return true;
                    }
                }
            }

            recursionStack.Remove(node);
            return false;
        }
    }
}