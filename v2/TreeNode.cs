using System;
using System.Collections.Generic;
using System.Linq;

namespace LegendaryTools.GraphV2
{
    public class TreeNode : Node, ITreeNode
    {
        public ITreeNode ParentNode { get;  set; }
        public List<ITreeNode> ChildNodes { get; set; }

        public TreeNode()
        {
            ChildNodes = new List<ITreeNode>();
        }

        public INodeConnection SetParent(ITreeNode newParent)
        {
            if (ParentNode != null)
            {
                INodeConnection existingConnection = FindConnectionBetweenNodes(ParentNode, this);
                if (existingConnection != null)
                {
                    RemoveConnection(existingConnection);
                }
            }
            ParentNode = newParent;
            return newParent?.ConnectTo(this, NodeConnectionDirection.Unidirectional);
        }
        
        public INodeConnection AddChild(ITreeNode newNode)
        {
            ChildNodes.Add(newNode);
            return this.ConnectTo(newNode, NodeConnectionDirection.Unidirectional);
        }

        public INodeConnection InsertChild(int index, ITreeNode newNode)
        {
            ChildNodes.Insert(index, newNode);
            return this.ConnectTo(newNode, NodeConnectionDirection.Unidirectional);
        }
        
        public List<INodeConnection> AddChildRange(IEnumerable<ITreeNode> collection)
        {
            List<INodeConnection> connections = new List<INodeConnection>();
            foreach (ITreeNode treeNode in collection)
            {
                connections.Add(AddChild(treeNode));
            }
            return connections;
        }
        
        public void RemoveRange(int index, int count)
        {
            for(int i = index; i < count; i++)
            {
                RemoveChild(ChildNodes[i]);
            }
        }
        
        public bool RemoveChild(ITreeNode nodeToRemove)
        {
            INodeConnection existingConnection = FindConnectionBetweenNodes(this, nodeToRemove);
            if (existingConnection != null)
            {
                RemoveConnection(existingConnection);
            }
            return ChildNodes.Remove(nodeToRemove);
        }

        public void ChildRemoveAt(int index)
        {
            INodeConnection existingConnection = FindConnectionBetweenNodes(this, ChildNodes[index]);
            if (existingConnection != null)
            {
                RemoveConnection(existingConnection);
            }
            ChildNodes.RemoveAt(index);
        }
        
        public bool Contains(ITreeNode nodeToRemove)
        {
            return ChildNodes.Contains(nodeToRemove);
        }

        public INodeConnection ConnectToParent(ITreeNode parent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (parent == this) throw new InvalidOperationException("A node cannot be its own parent.");
            if (IsAncestor(parent))
                throw new InvalidOperationException("Connecting to this parent would create a cycle.");

            // Disconnect from current parent if exists
            DisconnectFromParent();

            // Establish connection from parent to this node (Directed)
            INodeConnection connection = parent.ConnectTo(this, NodeConnectionDirection.Unidirectional);
            if (connection != null)
            {
                ParentNode = parent;
                parent.ChildNodes.Add(this);
            }

            return connection;
        }

        public virtual void DisconnectFromParent()
        {
            if (ParentNode != null)
            {
                INodeConnection connection =
                    Connections.FirstOrDefault(conn => conn.ToNode == this && conn.FromNode == ParentNode);
                if (connection != null)
                {
                    connection.Disconnect();
                    ParentNode.ChildNodes.Remove(this);
                    ParentNode = null;
                }
                Owner = null;
            }
        }

        private bool IsAncestor(ITreeNode potentialAncestor)
        {
            ITreeNode current = ParentNode;
            while (current != null)
            {
                if (current == potentialAncestor)
                    return true;
                current = current.ParentNode;
            }

            return false;
        }

        public override INode[] Neighbours
        {
            get
            {
                List<INode> neighbors = base.Neighbours.ToList();
                // For tree, only consider parent and children
                return neighbors.Where(n => ChildNodes.Contains(n as ITreeNode) || n == ParentNode).Distinct()
                    .ToArray();
            }
        }
    }
}