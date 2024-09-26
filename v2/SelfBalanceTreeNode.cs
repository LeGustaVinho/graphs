using System;
using System.Collections.Generic;

namespace LegendaryTools.GraphV2
{
    public class SelfBalanceTreeNode<T> : TreeNode, ISelfBalanceTreeNode<T> where T : IComparable<T>
    {
        // Implementing Key property from ISelfBalanceTreeNode<T>
        // For compliance, return the first key
        public T Key => Keys.Count > 0 ? Keys[0] : default(T);
        public List<T> Keys { get; set; }
        public bool IsLeaf => ChildNodes == null || ChildNodes.Count == 0;

        public SelfBalanceTreeNode()
        {
            Keys = new List<T>();
            ChildNodes = new List<ITreeNode>();
        }
        
        public SelfBalanceTreeNode(T key)
        {
            Keys = new List<T>();
            ChildNodes = new List<ITreeNode>();
            Keys.Add(key);
        }
    }
}