using System;
using NUnit.Framework;

namespace LegendaryTools.GraphV2.Tests
{
    namespace GraphTests
    {
        [TestFixture]
        public class GraphTests
        {
            private IGraph graph;
            private INode nodeA;
            private INode nodeB;
            private INode nodeC;
            private INode nodeD;

            [SetUp]
            public void SetUp()
            {
                graph = new Graph();
                nodeA = new Node();
                nodeA.Id = "NodeA";
                nodeB = new Node();
                nodeB.Id = "NodeB";
                nodeC = new Node();
                nodeC.Id = "NodeC";
                nodeD = new Node();
                nodeD.Id = "NodeD";
            }

            [Test]
            public void Graph_Creation_ShouldHaveUniqueId()
            {
                IGraph anotherGraph = new Graph();
                Assert.IsFalse(string.IsNullOrEmpty(graph.Id));
                Assert.IsFalse(string.IsNullOrEmpty(anotherGraph.Id));
                Assert.AreNotEqual(graph.Id, anotherGraph.Id);
            }

            [Test]
            public void Add_Node_ShouldContainNode()
            {
                graph.Add(nodeA);
                graph.Add(nodeB);

                Assert.IsTrue(graph.Contains(nodeA));
                Assert.IsTrue(graph.Contains(nodeB));
                Assert.AreEqual(2, graph.AllNodes.Length);
            }

            [Test]
            public void Add_NullNode_ShouldThrowArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => graph.Add(null));
            }

            [Test]
            public void Remove_Node_ShouldNoLongerContainNode()
            {
                graph.Add(nodeA);
                graph.Add(nodeB);

                bool removed = graph.Remove(nodeA);

                Assert.IsTrue(removed);
                Assert.IsFalse(graph.Contains(nodeA));
                Assert.AreEqual(1, graph.AllNodes.Length);
            }

            [Test]
            public void Remove_Node_ShouldDisconnectAllConnections()
            {
                graph.Add(nodeA);
                graph.Add(nodeB);
                graph.Add(nodeC);

                INodeConnection conn1 = nodeA.ConnectTo(nodeB, NodeConnectionDirection.Bidirectional);
                INodeConnection conn2 = nodeA.ConnectTo(nodeC, NodeConnectionDirection.Unidirectional);

                bool removed = graph.Remove(nodeA);

                Assert.IsTrue(removed);
                Assert.IsFalse(nodeB.Connections.Contains(conn1));
                Assert.IsFalse(nodeC.Connections.Contains(conn2));
                Assert.AreEqual(2, graph.AllNodes.Length);
            }

            [Test]
            public void Neighbours_InGraph_ShouldReturnCorrectNeighbours()
            {
                graph.Add(nodeA);
                graph.Add(nodeB);
                graph.Add(nodeC);

                nodeA.ConnectTo(nodeB, NodeConnectionDirection.Unidirectional);
                nodeA.ConnectTo(nodeC, NodeConnectionDirection.Bidirectional);

                INode[] neighbours = graph.Neighbours(nodeA);

                Assert.Contains(nodeB, neighbours);
                Assert.Contains(nodeC, neighbours);
                Assert.AreEqual(2, neighbours.Length);
            }

            [Test]
            public void Neighbours_NodeNotInGraph_ShouldThrowArgumentException()
            {
                graph.Add(nodeA);

                Assert.Throws<ArgumentException>(() => graph.Neighbours(nodeB));
            }

            [Test]
            public void AllNodesRecursive_ShouldReturnAllNodesInHierarchy()
            {
                graph.Add(nodeA);
                graph.Add(nodeB);

                IGraph childGraph = new Graph();
                childGraph.Add(nodeC);
                graph.ChildGraph = childGraph;

                IGraph grandChildGraph = new Graph();
                grandChildGraph.Add(nodeD);
                childGraph.ChildGraph = grandChildGraph;

                INode[] allNodes = graph.AllNodesRecursive;

                Assert.Contains(nodeA, allNodes);
                Assert.Contains(nodeB, allNodes);
                Assert.Contains(nodeC, allNodes);
                Assert.Contains(nodeD, allNodes);
                Assert.AreEqual(4, allNodes.Length);
            }

            [Test]
            public void GraphHierarchy_ShouldReturnCorrectHierarchy()
            {
                IGraph parentGraph = new Graph();
                IGraph childGraph = new Graph();
                IGraph grandChildGraph = new Graph();

                graph.ParentGraph = childGraph;
                childGraph.ParentGraph = parentGraph;
                grandChildGraph.ParentGraph = graph;

                IGraph[] hierarchy = grandChildGraph.GraphHierarchy;

                Assert.AreEqual(2, hierarchy.Length);
                Assert.AreEqual(parentGraph, hierarchy[0]);
                Assert.AreEqual(childGraph, hierarchy[1]);
            }

            [Test]
            public void IsAcyclic_UndirectedGraph_NoCycle_ShouldBeAcyclic()
            {
                graph.Add(nodeA);
                graph.Add(nodeB);
                graph.Add(nodeC);

                nodeA.ConnectTo(nodeB, NodeConnectionDirection.Bidirectional);
                nodeB.ConnectTo(nodeC, NodeConnectionDirection.Bidirectional);

                Assert.IsTrue(graph.IsAcyclic);
                Assert.IsFalse(graph.IsCyclic);
            }

            [Test]
            public void IsAcyclic_UndirectedGraph_WithCycle_ShouldBeCyclic()
            {
                graph.Add(nodeA);
                graph.Add(nodeB);
                graph.Add(nodeC);

                nodeA.ConnectTo(nodeB, NodeConnectionDirection.Bidirectional);
                nodeB.ConnectTo(nodeC, NodeConnectionDirection.Bidirectional);
                nodeC.ConnectTo(nodeA, NodeConnectionDirection.Bidirectional);

                Assert.IsFalse(graph.IsAcyclic);
                Assert.IsTrue(graph.IsCyclic);
            }

            [Test]
            public void IsDirectedAcyclic_DirectedGraph_NoCycle_ShouldBeAcyclic()
            {
                graph.Add(nodeA);
                graph.Add(nodeB);
                graph.Add(nodeC);

                nodeA.ConnectTo(nodeB, NodeConnectionDirection.Unidirectional);
                nodeB.ConnectTo(nodeC, NodeConnectionDirection.Unidirectional);

                Assert.IsTrue(graph.IsDirectedAcyclic);
                Assert.IsFalse(graph.IsDirectedCyclic);
            }

            [Test]
            public void IsDirectedAcyclic_DirectedGraph_WithCycle_ShouldBeCyclic()
            {
                graph.Add(nodeA);
                graph.Add(nodeB);
                graph.Add(nodeC);

                nodeA.ConnectTo(nodeB, NodeConnectionDirection.Unidirectional);
                nodeB.ConnectTo(nodeC, NodeConnectionDirection.Unidirectional);
                nodeC.ConnectTo(nodeA, NodeConnectionDirection.Unidirectional);

                Assert.IsFalse(graph.IsDirectedAcyclic);
                Assert.IsTrue(graph.IsDirectedCyclic);
            }

            [Test]
            public void IsDirectedCyclic_MixedGraph_ShouldDetectDirectedCyclesOnly()
            {
                graph.Add(nodeA);
                graph.Add(nodeB);
                graph.Add(nodeC);
                graph.Add(nodeD);

                nodeA.ConnectTo(nodeB, NodeConnectionDirection.Unidirectional);
                nodeB.ConnectTo(nodeC, NodeConnectionDirection.Bidirectional);
                nodeC.ConnectTo(nodeA, NodeConnectionDirection.Unidirectional); // Directed cycle A -> B -> C -> A
                nodeC.ConnectTo(nodeD, NodeConnectionDirection.Bidirectional);

                Assert.IsFalse(graph.IsAcyclic);
                Assert.IsTrue(graph.IsCyclic);
                Assert.IsTrue(graph.IsDirectedCyclic);
                Assert.IsFalse(graph.IsDirectedAcyclic);
            }

            [Test]
            public void AllNodes_ShouldReturnOnlyNodesInGraph()
            {
                graph.Add(nodeA);
                graph.Add(nodeB);

                INode[] allNodes = graph.AllNodes;

                Assert.Contains(nodeA, allNodes);
                Assert.Contains(nodeB, allNodes);
                Assert.AreEqual(2, allNodes.Length);
            }

            [Test]
            public void Contains_ShouldReturnTrueIfNodeExists()
            {
                graph.Add(nodeA);
                Assert.IsTrue(graph.Contains(nodeA));
                Assert.IsFalse(graph.Contains(nodeB));
            }

            [Test]
            public void RemovingNonExistentNode_ShouldReturnFalse()
            {
                graph.Add(nodeA);
                bool removed = graph.Remove(nodeB);
                Assert.IsFalse(removed);
            }

            [Test]
            public void Graph_IsDirected_ShouldReflectIfAnyConnectionIsDirected()
            {
                graph.Add(nodeA);
                graph.Add(nodeB);
                graph.Add(nodeC);

                nodeA.ConnectTo(nodeB, NodeConnectionDirection.Bidirectional);
                Assert.IsFalse(graph.IsDirectedAcyclic);
                Assert.IsFalse(graph.IsDirectedCyclic);

                nodeB.ConnectTo(nodeC, NodeConnectionDirection.Unidirectional);
                Assert.IsTrue(graph.IsDirectedAcyclic);
                Assert.IsFalse(graph.IsDirectedCyclic);
            }
        }
    }
}