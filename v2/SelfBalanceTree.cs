using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LegendaryTools.GraphV2
{
    public class SelfBalanceTree<T> : Tree, ISelfBalanceTree<T> where T : IComparable<T>
    {
        public int Degree { get; }
        public IComparer<T> OverrideComparer { get; set; }

        public SelfBalanceTree(int degree)
        {
            if (degree < 2) throw new ArgumentException("Degree must be at least 2.");
            Degree = degree;
        }

        public void AddSelfBalanceTreeNode(ISelfBalanceTreeNode<T> newNode)
        {
            if (newNode == null) throw new ArgumentNullException(nameof(newNode));
            T key = newNode.Key;

            if (RootNode == null)
            {
                RootNode = newNode;
                Add(newNode);
            }
            else
            {
                SelfBalanceTreeNode<T> root = (SelfBalanceTreeNode<T>)RootNode;
                if (root.Keys.Count == 2 * Degree - 1)
                {
                    SelfBalanceTreeNode<T> newRoot = new SelfBalanceTreeNode<T>();
                    RootNode = newRoot;
                    Add(newRoot);
                    newRoot.ChildNodes.Add(root);
                    root.ParentNode = newRoot;
                    SplitChild(newRoot, 0, root);
                    InsertNonFull(newRoot, key);
                }
                else
                {
                    InsertNonFull(root, key);
                }
            }
        }

        //V1
        // private void SplitChild(SelfBalanceTreeNode<T> parentNode, int index, SelfBalanceTreeNode<T> fullChildNode)
        // {
        //     int t = Degree;
        //     SelfBalanceTreeNode<T> newChild = new SelfBalanceTreeNode<T>();
        //     Add(newChild);
        //     newChild.ParentNode = parentNode;
        //
        //     // Move keys
        //     for (int j = 0; j < t - 1; j++)
        //     {
        //         newChild.Keys.Add(fullChildNode.Keys[t]);
        //         fullChildNode.Keys.RemoveAt(t);
        //     }
        //
        //     // Move child pointers if not leaf
        //     if (!fullChildNode.IsLeaf)
        //         for (int j = 0; j < t; j++)
        //         {
        //             SelfBalanceTreeNode<T> child = (SelfBalanceTreeNode<T>)fullChildNode.ChildNodes[t];
        //             fullChildNode.ChildNodes.RemoveAt(t);
        //             newChild.ChildNodes.Add(child);
        //             child.ParentNode = newChild;
        //         }
        //
        //     // Insert new child into parent
        //     parentNode.ChildNodes.Insert(index + 1, newChild);
        //
        //     // Move middle key up to parent
        //     parentNode.Keys.Insert(index, fullChildNode.Keys[t - 1]);
        //     fullChildNode.Keys.RemoveAt(t - 1);
        // }

        //V2
        private void SplitChild(SelfBalanceTreeNode<T> parentNode, int index, SelfBalanceTreeNode<T> fullChildNode)
        {
            int t = Degree;
            SelfBalanceTreeNode<T> newChild = new SelfBalanceTreeNode<T>();
            newChild.ParentNode = parentNode;

            // Copy the last t - 1 keys of fullChildNode to newChild
            newChild.Keys.AddRange(fullChildNode.Keys.GetRange(t, t - 1));
            fullChildNode.Keys.RemoveRange(t, t - 1);

            // If fullChildNode is not a leaf, move its last t children to newChild
            if (!fullChildNode.IsLeaf)
            {
                newChild.ChildNodes.AddRange(fullChildNode.ChildNodes.GetRange(t, t));
                foreach (ITreeNode child in newChild.ChildNodes) child.ParentNode = newChild;
                fullChildNode.ChildNodes.RemoveRange(t, t);
            }

            // Insert newChild into parentNode's child list
            parentNode.ChildNodes.Insert(index + 1, newChild);

            // Move the middle key of fullChildNode up to parentNode
            parentNode.Keys.Insert(index, fullChildNode.Keys[t - 1]);
            fullChildNode.Keys.RemoveAt(t - 1);
        }

        private void InsertNonFull(SelfBalanceTreeNode<T> node, T key)
        {
            int i = node.Keys.Count - 1;
            if (node.IsLeaf)
            {
                // Insert key into node
                node.Keys.Add(default); // Temporary space for new key
                while (i >= 0 && CompareKeys(key, node.Keys[i]) < 0)
                {
                    node.Keys[i + 1] = node.Keys[i];
                    i--;
                }

                node.Keys[i + 1] = key;
            }
            else
            {
                // Move to correct child node
                while (i >= 0 && CompareKeys(key, node.Keys[i]) < 0) i--;
                i++;
                SelfBalanceTreeNode<T> child = node.ChildNodes[i];
                if (child.Keys.Count == 2 * Degree - 1)
                {
                    SplitChild(node, i, child);
                    if (CompareKeys(key, node.Keys[i]) > 0) i++;
                }

                InsertNonFull(node.ChildNodes[i], key);
            }
        }

        private int CompareKeys(T x, T y)
        {
            if (OverrideComparer != null) return OverrideComparer.Compare(x, y);
            return x.CompareTo(y);
        }

        public bool RemoveSelfBalanceTreeNode(ISelfBalanceTreeNode<T> node, out ISelfBalanceTreeNode<T>[] removedNodes)
        {
            removedNodes = null;
            if (node == null)
                return false;
            T key = node.Key;

            if (!ContainsKey(key))
                return false;

            SelfBalanceTreeNode<T> root = (SelfBalanceTreeNode<T>)RootNode;
            List<SelfBalanceTreeNode<T>> removedNodesList = new List<SelfBalanceTreeNode<T>>();

            DeleteKey(root, key, removedNodesList);

            // After deletion, if root has 0 keys and has a child, make the child the new root
            if (root.Keys.Count == 0)
            {
                if (!root.IsLeaf)
                {
                    RootNode = root.ChildNodes[0];
                    RootNode.ParentNode = null;
                }
                else
                {
                    RootNode = null;
                }

                Remove(root);
                removedNodesList.Add(root);
            }

            removedNodes = removedNodesList.Cast<ISelfBalanceTreeNode<T>>().ToArray();
            return true;
        }

        // Helper methods for deletion

        private bool ContainsKey(T key)
        {
            return SearchKey((SelfBalanceTreeNode<T>)RootNode, key) != null;
        }

        private SelfBalanceTreeNode<T> SearchKey(SelfBalanceTreeNode<T> node, T key)
        {
            int i = 0;
            while (i < node.Keys.Count && CompareKeys(key, node.Keys[i]) > 0) i++;

            if (i < node.Keys.Count && CompareKeys(key, node.Keys[i]) == 0)
                return node;

            if (node.IsLeaf)
                return null;
            return SearchKey(node.ChildNodes[i], key);
        }

        private void DeleteKey(SelfBalanceTreeNode<T> node, T key, List<SelfBalanceTreeNode<T>> removedNodesList)
        {
            int idx = node.Keys.FindIndex(k => CompareKeys(k, key) >= 0);

            if (idx < node.Keys.Count && CompareKeys(node.Keys[idx], key) == 0)
            {
                if (node.IsLeaf)
                    // Case 1: Key is in leaf node
                    node.Keys.RemoveAt(idx);
                else
                    // Case 2: Key is in internal node
                    DeleteInternalKey(node, key, idx, removedNodesList);
            }
            else
            {
                if (node.IsLeaf)
                    // Key not found in tree
                    return;

                bool flag = idx == node.Keys.Count;

                SelfBalanceTreeNode<T> child = node.ChildNodes[idx];
                if (child.Keys.Count < Degree) Fill(node, idx, removedNodesList);

                if (flag && idx > node.Keys.Count)
                    DeleteKey(node.ChildNodes[idx - 1], key, removedNodesList);
                else
                    DeleteKey(node.ChildNodes[idx], key, removedNodesList);
            }
        }

        private void DeleteInternalKey(SelfBalanceTreeNode<T> node, T key, int idx,
            List<SelfBalanceTreeNode<T>> removedNodesList)
        {
            T k = node.Keys[idx];

            SelfBalanceTreeNode<T> predChild = node.ChildNodes[idx];
            if (predChild.Keys.Count >= Degree)
            {
                T predKey = GetPredecessor(predChild);
                node.Keys[idx] = predKey;
                DeleteKey(predChild, predKey, removedNodesList);
            }
            else
            {
                SelfBalanceTreeNode<T> succChild = node.ChildNodes[idx + 1];
                if (succChild.Keys.Count >= Degree)
                {
                    T succKey = GetSuccessor(succChild);
                    node.Keys[idx] = succKey;
                    DeleteKey(succChild, succKey, removedNodesList);
                }
                else
                {
                    Merge(node, idx, removedNodesList);
                    DeleteKey(predChild, key, removedNodesList);
                }
            }
        }

        private T GetPredecessor(SelfBalanceTreeNode<T> node)
        {
            while (!node.IsLeaf) node = node.ChildNodes[node.ChildNodes.Count - 1];
            return node.Keys[node.Keys.Count - 1];
        }

        private T GetSuccessor(SelfBalanceTreeNode<T> node)
        {
            while (!node.IsLeaf) node = node.ChildNodes[0];
            return node.Keys[0];
        }

        private void Fill(SelfBalanceTreeNode<T> node, int idx, List<SelfBalanceTreeNode<T>> removedNodesList)
        {
            if (idx != 0 && node.ChildNodes[idx - 1].Keys.Count >= Degree)
            {
                BorrowFromPrev(node, idx);
            }
            else if (idx != node.Keys.Count &&
                     node.ChildNodes[idx + 1].Keys.Count >= Degree)
            {
                BorrowFromNext(node, idx);
            }
            else
            {
                if (idx != node.Keys.Count)
                    Merge(node, idx, removedNodesList);
                else
                    Merge(node, idx - 1, removedNodesList);
            }
        }

        private void BorrowFromPrev(SelfBalanceTreeNode<T> node, int idx)
        {
            SelfBalanceTreeNode<T> child = node.ChildNodes[idx];
            SelfBalanceTreeNode<T> sibling = node.ChildNodes[idx - 1];

            child.Keys.Insert(0, node.Keys[idx - 1]);

            if (!sibling.IsLeaf)
            {
                SelfBalanceTreeNode<T> lastChild =
                    sibling.ChildNodes[sibling.ChildNodes.Count - 1];
                sibling.ChildNodes.RemoveAt(sibling.ChildNodes.Count - 1);
                child.ChildNodes.Insert(0, lastChild);
                lastChild.ParentNode = child;
            }

            node.Keys[idx - 1] = sibling.Keys[sibling.Keys.Count - 1];
            sibling.Keys.RemoveAt(sibling.Keys.Count - 1);
        }

        private void BorrowFromNext(SelfBalanceTreeNode<T> node, int idx)
        {
            SelfBalanceTreeNode<T> child = node.ChildNodes[idx];
            SelfBalanceTreeNode<T> sibling = node.ChildNodes[idx + 1];

            child.Keys.Add(node.Keys[idx]);

            if (!sibling.IsLeaf)
            {
                SelfBalanceTreeNode<T> firstChild = sibling.ChildNodes[0];
                sibling.ChildNodes.RemoveAt(0);
                child.ChildNodes.Add(firstChild);
                firstChild.ParentNode = child;
            }

            node.Keys[idx] = sibling.Keys[0];
            sibling.Keys.RemoveAt(0);
        }

        private void Merge(SelfBalanceTreeNode<T> node, int idx, List<SelfBalanceTreeNode<T>> removedNodesList)
        {
            SelfBalanceTreeNode<T> child = node.ChildNodes[idx];
            SelfBalanceTreeNode<T> sibling = node.ChildNodes[idx + 1];

            child.Keys.Add(node.Keys[idx]);
            child.Keys.AddRange(sibling.Keys);

            if (!child.IsLeaf)
                foreach (SelfBalanceTreeNode<T> c in sibling.ChildNodes)
                {
                    child.ChildNodes.Add(c);
                    c.ParentNode = child;
                }

            node.Keys.RemoveAt(idx);
            node.ChildNodes.RemoveAt(idx + 1);

            Remove(sibling);
            removedNodesList.Add(sibling);
        }

        public override void VisualizeTree(bool showDepth = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("B-Tree");

            if (showDepth)
                VisualizeNode((SelfBalanceTreeNode<T>)RootNode, "", true, sb, 0);
            else
                VisualizeNode((SelfBalanceTreeNode<T>)RootNode, "", true, sb);

            Debug.Log(sb.ToString());
        }

        private void VisualizeNode(SelfBalanceTreeNode<T> node, string indent, bool last, StringBuilder sb)
        {
            if (node == null)
                return;

            // Print the current node's keys with appropriate indentation
            sb.AppendLine(indent + (last ? "└─ " : "├─ ") + $"[Keys: {string.Join(", ", node.Keys)}]");

            // Update the indentation for child nodes
            indent += last ? "   " : "│  ";

            // Recursively print all child nodes
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                SelfBalanceTreeNode<T> childNode = node.ChildNodes[i];
                VisualizeNode(childNode, indent, i == node.ChildNodes.Count - 1, sb);
            }
        }

        private void VisualizeNode(SelfBalanceTreeNode<T> node, string indent, bool last, StringBuilder sb, int depth)
        {
            if (node == null)
                return;

            sb.AppendLine($"{indent}{(last ? "└──" : "├──")} [Depth: {depth}] [Keys: {string.Join(", ", node.Keys)}]");
            indent += last ? "    " : "│   ";

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                SelfBalanceTreeNode<T> childNode = node.ChildNodes[i];
                VisualizeNode(childNode, indent, i == node.ChildNodes.Count - 1, sb, depth + 1);
            }
        }
    }
}