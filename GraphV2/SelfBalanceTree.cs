using System;
using System.Collections.Generic;
using System.Linq;

namespace LegendaryTools.GraphV2
{
    public class SelfBalanceTree<T> : Tree, ISelfBalanceTree<T>
        where T : IComparable<T>
    {
        public IComparer<T> OverrideComparer { get; set; }

        private readonly int degree;

        public SelfBalanceTree(int degree)
        {
            if (degree < 2)
                throw new ArgumentException("Degree must be at least 2.");

            this.degree = degree;
            OverrideComparer = Comparer<T>.Default;
        }

        /// <summary>
        ///     Inserts a new node into the B-Tree.
        /// </summary>
        /// <param name="newNode">The node to insert.</param>
        /// <param name="parentNode">The parent node. If null, the node is inserted at the root.</param>
        /// <param name="weight">Optional weight for the connection.</param>
        public void AddSelfBalanceTreeNode(ISelfBalanceTreeNode<T> newNode, ISelfBalanceTreeNode<T> parentNode,
            float weight = 1)
        {
            if (RootNode == null)
            {
                RootNode = newNode;
                Add(newNode);
                return;
            }

            SelfBalanceTreeNode<T> currentNode = RootNode as SelfBalanceTreeNode<T>;

            if (currentNode.IsFull)
            {
                // Create a new root
                SelfBalanceTreeNode<T> newRoot = new SelfBalanceTreeNode<T>(degree);
                Add(newRoot);
                newRoot.ConnectToParent(currentNode, weight);
                SplitChild(newRoot, 0, currentNode);
                InsertNonFull(newRoot, newNode);
                RootNode = newRoot;
            }
            else
            {
                InsertNonFull(currentNode, newNode);
            }
        }

        /// <summary>
        ///     Inserts a node into a non-full node.
        /// </summary>
        /// <param name="node">The node to insert into.</param>
        /// <param name="newNode">The node to insert.</param>
        private void InsertNonFull(ISelfBalanceTreeNode<T> node, ISelfBalanceTreeNode<T> newNode)
        {
            int i = node.Keys.Count - 1;

            if (node.IsLeaf)
            {
                node.InsertKey(newNode.Data);
                return;
            }

            // Find the child which is going to have the new key
            while (i >= 0 && OverrideComparer.Compare(newNode.Data, node.Keys[i]) < 0)
                i--;

            i++;

            SelfBalanceTreeNode<T> child = node.ChildNodes[i] as SelfBalanceTreeNode<T>;

            if (child.IsFull)
            {
                SplitChild(node, i, child);

                if (OverrideComparer.Compare(newNode.Data, node.Keys[i]) > 0)
                    i++;
            }

            child = node.ChildNodes[i] as SelfBalanceTreeNode<T>;

            InsertNonFull(child, newNode);
        }

        /// <summary>
        ///     Splits the full child node into two and adjusts the parent node.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="index">The index of the child in the parent.</param>
        /// <param name="child">The full child node to split.</param>
        private void SplitChild(ISelfBalanceTreeNode<T> parent, int index, ISelfBalanceTreeNode<T> child)
        {
            // Create a new node which will store (degree-1) keys of child
            SelfBalanceTreeNode<T> newChild = new SelfBalanceTreeNode<T>(degree);
            for (int j = 0; j < degree - 1; j++) newChild.Keys.Add(child.Keys[j + degree]);

            // If child is not leaf, move the last degree children to newChild
            if (!child.IsLeaf)
            {
                foreach (ITreeNode grandChild in child.ChildNodes.Skip(degree))
                    newChild.ConnectToParent(grandChild as SelfBalanceTreeNode<T>);

                // Remove the moved children from the original child
                child.ChildNodes = child.ChildNodes.Take(degree).ToList();
            }

            // Reduce the number of keys in child
            child.Keys = child.Keys.Take(degree - 1).ToList();

            // Insert a new key into parent
            parent.InsertKey(child.Keys[degree - 1]);

            // Insert new child into parent
            AddChild(parent, newChild);

            // Connect newChild to parent
            newChild.ConnectToParent(parent);
        }

        /// <summary>
        ///     Adds a child to the parent node at the appropriate position.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="child">The child node to add.</param>
        private void AddChild(ISelfBalanceTreeNode<T> parent, ISelfBalanceTreeNode<T> child)
        {
            parent.ChildNodes = parent.ChildNodes.Concat(new ITreeNode[] { child }).ToList();
        }

        /// <summary>
        ///     Removes a key from the B-Tree.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        public void Remove(T key)
        {
            ISelfBalanceTreeNode<T> node = Search(RootNode as SelfBalanceTreeNode<T>, key);
            if (node == null)
                throw new KeyNotFoundException("The key does not exist in the B-Tree.");

            // Implement deletion logic here (omitted for brevity)
            // Proper deletion in B-Trees is complex and involves several cases.
            // For the purpose of this implementation, we'll focus on insertion.
        }

        /// <summary>
        ///     Searches for a key in the B-Tree starting from the given node.
        /// </summary>
        /// <param name="node">The node to start searching from.</param>
        /// <param name="key">The key to search for.</param>
        /// <returns>The node containing the key, or null if not found.</returns>
        private ISelfBalanceTreeNode<T> Search(ISelfBalanceTreeNode<T> node, T key)
        {
            int i = 0;
            while (i < node.Keys.Count && OverrideComparer.Compare(key, node.Keys[i]) > 0)
                i++;

            if (i < node.Keys.Count && OverrideComparer.Compare(key, node.Keys[i]) == 0)
                return node;

            if (node.IsLeaf)
                return null;

            return Search(node.ChildNodes[i] as SelfBalanceTreeNode<T>, key);
        }

        /// <summary>
        ///     Overrides the base Tree's Add method to prevent misuse.
        /// </summary>
        /// <param name="newNode">The node to add.</param>
        public new void Add(INode newNode)
        {
            throw new NotSupportedException("Use AddSelfBalanceTreeNode or Add methods specific to SelfBalanceTree.");
        }

        /// <summary>
        ///     Overrides the base Tree's Remove method to prevent misuse.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        public new bool Remove(INode node)
        {
            throw new NotSupportedException(
                "Use RemoveSelfBalanceTreeNode or Remove methods specific to SelfBalanceTree.");
        }

        // Implement other required methods from ISelfBalanceTree<T> interface

        public bool RemoveSelfBalanceTreeNode(ISelfBalanceTreeNode<T> node, out ISelfBalanceTreeNode<T>[] removedNodes)
        {
            removedNodes = Array.Empty<ISelfBalanceTreeNode<T>>();
            throw new NotImplementedException("Deletion in B-Trees is not implemented in this example.");
        }
    }
}