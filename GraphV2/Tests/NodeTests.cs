using NUnit.Framework;
using System;
using System.Linq;

namespace LegendaryTools.GraphV2.Tests
{
    [TestFixture]
    public class NodeTests
    {
        private INode nodeA;
        private INode nodeB;
        private INode nodeC;

        [SetUp]
        public void SetUp()
        {
            nodeA = new Node();
            nodeA.Id = "NodeA";
            nodeB = new Node();
            nodeB.Id = "NodeB";
            nodeC = new Node();
            nodeC.Id = "NodeC";
        }

        [Test]
        public void Node_Creation_ShouldHaveUniqueId()
        {
            Assert.IsFalse(string.IsNullOrEmpty(nodeA.Id));
            Assert.IsFalse(string.IsNullOrEmpty(nodeB.Id));
            Assert.AreNotEqual(nodeA.Id, nodeB.Id);
        }

        [Test]
        public void ConnectTo_DirectedConnection_ShouldAddOutboundAndInboundConnections()
        {
            var connection = nodeA.ConnectTo(nodeB, NodeConnectionDirection.Unidirectional, 2.5f);

            Assert.Contains(connection, nodeA.OutboundConnections.ToList());
            Assert.Contains(connection, nodeB.InboundConnections.ToList());
            Assert.AreEqual(1, nodeA.Count);
            Assert.AreEqual(1, nodeB.Count);
        }

        [Test]
        public void ConnectTo_UndirectedConnection_ShouldAddConnectionsToBothNodes()
        {
            var connection = nodeA.ConnectTo(nodeB, NodeConnectionDirection.Bidirectional, 1.0f);

            Assert.Contains(connection, nodeA.Connections);
            Assert.Contains(connection, nodeB.Connections);
            Assert.AreEqual(1, nodeA.Count);
            Assert.AreEqual(1, nodeB.Count);
        }

        [Test]
        public void ConnectTo_SameNode_ShouldThrowInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => nodeA.ConnectTo(nodeA, NodeConnectionDirection.Unidirectional));
        }

        [Test]
        public void ConnectTo_NullNode_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => nodeA.ConnectTo(null, NodeConnectionDirection.Unidirectional));
        }

        [Test]
        public void ConnectTo_ExistingConnection_ShouldNotCreateDuplicate()
        {
            var connection1 = nodeA.ConnectTo(nodeB, NodeConnectionDirection.Unidirectional);
            var connection2 = nodeA.ConnectTo(nodeB, NodeConnectionDirection.Unidirectional);

            Assert.AreEqual(connection1, connection2);
            Assert.AreEqual(1, nodeA.Count);
            Assert.AreEqual(1, nodeB.Count);
        }

        [Test]
        public void RemoveConnection_ShouldRemoveConnectionFromBothNodes()
        {
            var connection = nodeA.ConnectTo(nodeB, NodeConnectionDirection.Bidirectional);

            bool removed = nodeA.RemoveConnection(connection);

            Assert.IsTrue(removed);
            Assert.IsFalse(nodeA.Connections.Contains(connection));
            Assert.IsFalse(nodeB.Connections.Contains(connection));
            Assert.AreEqual(0, nodeA.Count);
            Assert.AreEqual(0, nodeB.Count);
        }

        [Test]
        public void Neighbours_ShouldReturnCorrectNeighbours_Directed()
        {
            nodeA.ConnectTo(nodeB, NodeConnectionDirection.Unidirectional);
            nodeA.ConnectTo(nodeC, NodeConnectionDirection.Unidirectional);

            var neighbours = nodeA.Neighbours;

            Assert.Contains(nodeB, neighbours);
            Assert.Contains(nodeC, neighbours);
            Assert.AreEqual(2, neighbours.Length);
        }

        [Test]
        public void Neighbours_ShouldReturnCorrectNeighbours_Undirected()
        {
            var aBConn = nodeA.ConnectTo(nodeB, NodeConnectionDirection.Bidirectional);
            var acConn = nodeA.ConnectTo(nodeC, NodeConnectionDirection.Bidirectional);

            aBConn.Id = "A <--> B";
            acConn.Id = "A <--> C";

            var neighbours = nodeA.Neighbours;

            Assert.Contains(nodeB, neighbours);
            Assert.Contains(nodeC, neighbours);
            Assert.AreEqual(2, neighbours.Length);
        }

        [Test]
        public void Count_ShouldReflectNumberOfConnections()
        {
            Assert.AreEqual(0, nodeA.Count);
            nodeA.ConnectTo(nodeB, NodeConnectionDirection.Unidirectional);
            Assert.AreEqual(1, nodeA.Count);
            nodeA.ConnectTo(nodeC, NodeConnectionDirection.Bidirectional);
            Assert.AreEqual(2, nodeA.Count);
        }
    }
}
