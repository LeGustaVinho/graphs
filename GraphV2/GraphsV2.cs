﻿using System;
using System.Collections.Generic;

namespace LegendaryTools.GraphV2
{
    public interface ISelfBalanceTreeNode<T> : ITreeNode
        where T : IComparable<T>
    {
        public T Data { get; }
        public int Degree { get; } // Minimum degree (defines the range for number of keys)
        public List<T> Keys { get; set; }
        public bool IsFull { get; }
        bool IsLeaf { get; }

        void InsertKey(T key);
    }
    
    public interface ISelfBalanceTree<T> : ITree
        where T : IComparable<T>
    {
        public IComparer<T> OverrideComparer { get; set; }
        
        void AddSelfBalanceTreeNode(ISelfBalanceTreeNode<T> newNode, ISelfBalanceTreeNode<T> parentNode, float weight = 1); // Insert a new node in the B-tree
        bool RemoveSelfBalanceTreeNode(ISelfBalanceTreeNode<T> node, out ISelfBalanceTreeNode<T>[] removedNodes); // Remove a node from the B-tree
    }

    public interface IBinaryTreeNode : ITreeNode
    {
        public ITreeNode Left { get;  }
        public ITreeNode Right { get; }
    }
    
    public interface IBinaryTree : ITree
    {
        void AddTreeNode(IBinaryTreeNode newNode, IBinaryTreeNode parentNode, float weight = 1); //Adds a node to the Binary Tree, validating the graph as Binary Tree
        IBinaryTreeNode BinarySearch(Predicate<INode> predicate);
    }

    public interface ITree : IGraph
    {
        ITreeNode RootNode { get; }
        public int Height { get; }
        public int Width { get; }
        void AddTreeNode(ITreeNode newNode, ITreeNode parentNode, float weight = 1); //Adds a node to the tree, validating the graph remains directed acyclic tree
        bool RemoveTreeNode(ITreeNode node, out ITreeNode[] removedNodes);
        public ITreeNode DepthFirstSearch(Predicate<INode> predicate);
        public ITreeNode HeightFirstSearch(Predicate<INode> predicate);
        List<ITreeNode> DepthFirstTraverse();
        List<ITreeNode> HeightFirstTraverse();
    }
    
    public interface ITreeNode : INode
    {
        public ITreeNode ParentNode { get; }
        public List<ITreeNode> ChildNodes { get; set; }
        INodeConnection ConnectToParent(ITreeNode parent, float weight); //Connects this node to a parent node in a tree structure. Ensures that the connection is directed from parent to child.
        void DisconnectFromParent(); //Disconnects this node from its parent.
    }
    
    public interface IGraph
    {
        public string Id { get; set; } //Guid
        public bool IsDirectedAcyclic { get; } //Also checks NodeConnectionDirection
        public bool IsDirectedCyclic { get; } //Also checks NodeConnectionDirection
        public bool IsAcyclic { get; } //Dont check NodeConnectionDirection
        public bool IsCyclic { get; } //Dont check NodeConnectionDirection
        IGraph ParentGraph { get; set; }
        IGraph ChildGraph { get; set; }
        IGraph[] GraphHierarchy { get; } //Returns the hierarchy of this graph based on ParentGraph looking recursive until ParentGraph is null, in order
        INode[] AllNodes { get; } //Return all nodes only in this graph
        INode[] AllNodesRecursive { get; } //Return all nodes in this graph recursive in ChildGraph nodes
        void Add(INode newNode);
        bool Remove(INode node);
        bool Contains(INode node);
        INode[] Neighbours(INode node);
    }
    
    public interface INode
    {
        public string Id { get; set; } //Guid
        INode[] Neighbours { get; }
        List<INodeConnection> Connections { get; }
        INodeConnection[] OutboundConnections { get; }
        INodeConnection[] InboundConnections { get; }
        int Count { get; }
        INodeConnection ConnectTo(INode to, NodeConnectionDirection direction, float weight = 1);
        bool RemoveConnection(INodeConnection nodeConnection);
    }
    
    public interface INodeConnection
    {
        public string Id { get; set; } //Guid
        public INode FromNode { get; set; }
        public INode ToNode { get; set; }
        NodeConnectionDirection Direction { get; set; }
        void Disconnect();
    }
}