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
        
        // Overriding ChildNodes to be of type List<SelfBalanceTreeNode<T>>
        public new List<SelfBalanceTreeNode<T>> ChildNodes { get; set; }

        // Overriding ParentNode to be of type SelfBalanceTreeNode<T>
        public new SelfBalanceTreeNode<T> ParentNode { get; set; }

        public SelfBalanceTreeNode()
        {
            Keys = new List<T>();
            ChildNodes = new List<SelfBalanceTreeNode<T>>();
        }
        
        public SelfBalanceTreeNode(T key)
        {
            Keys = new List<T>();
            ChildNodes = new List<SelfBalanceTreeNode<T>>();
            Keys.Add(key);
        }
    }
}