using System;
using System.Collections.Generic;

namespace LegendaryTools.GraphV2
{
    public class SelfBalanceTreeNode<T> : TreeNode, ISelfBalanceTreeNode<T> where T : IComparable<T>
    {
        public T Data { get; }
        public List<T> Keys { get; set; }
        public int Degree { get; set; } // Minimum degree (t) of B-Tree
        public bool IsLeaf => ChildNodes == null || ChildNodes.Count == 0;

        public SelfBalanceTreeNode(int degree)
        {
            Degree = degree;
            Keys = new List<T>();
            ChildNodes = new List<ITreeNode>();
        }

        // Implementing Key property from ISelfBalanceTreeNode<T>
        public T Key
        {
            get
            {
                // For compliance, return the first key
                if (Keys.Count > 0)
                    return Keys[0];
                else
                    return default(T);
            }
        }
    }
}