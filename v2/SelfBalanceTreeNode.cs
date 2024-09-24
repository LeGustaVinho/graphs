using System;
using System.Collections.Generic;

namespace LegendaryTools.GraphV2
{
    public class SelfBalanceTreeNode<T> : TreeNode, ISelfBalanceTreeNode<T>
        where T : IComparable<T>
    {
        public T Data { get; private set; }
        public int Degree { get; private set; }
        public List<T> Keys { get; set; }

        public SelfBalanceTreeNode(int degree, T data = default)
        {
            Degree = degree;
            Keys = new List<T>();
            Data = data;
        }

        /// <summary>
        /// Inserts a key into the node in a sorted manner.
        /// </summary>
        /// <param name="key">The key to insert.</param>
        public void InsertKey(T key)
        {
            if (Keys.Contains(key))
                throw new InvalidOperationException("Duplicate keys are not allowed in B-Tree.");

            Keys.Add(key);
            Keys.Sort();
        }

        /// <summary>
        /// Removes a key from the node.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        public bool RemoveKey(T key)
        {
            return Keys.Remove(key);
        }

        /// <summary>
        /// Determines if the node is full.
        /// </summary>
        public bool IsFull => Keys.Count == (2 * Degree - 1);

        /// <summary>
        /// Determines if the node is a leaf.
        /// </summary>
        public bool IsLeaf => ChildNodes.Count == 0;

        /// <summary>
        /// Finds the index of the first key greater than or equal to the given key.
        /// </summary>
        /// <param name="key">The key to find.</param>
        /// <returns>The index.</returns>
        public int FindKeyIndex(T key)
        {
            int index = 0;
            while (index < Keys.Count && Keys[index].CompareTo(key) < 0)
                index++;
            return index;
        }
    }
}