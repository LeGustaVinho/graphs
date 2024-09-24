using System;

namespace LegendaryTools.GraphV2
{
    public enum NodeConnectionDirection
    {
        Unidirectional,
        Bidirectional
    }
    
    public class NodeConnection : INodeConnection
    {
        public string Id { get; set; }
        public INode FromNode { get; set; }
        public INode ToNode { get; set; }
        public NodeConnectionDirection Direction { get; set; }
        public float Weight { get; set; }

        public NodeConnection(INode fromNode, INode toNode, NodeConnectionDirection direction, float weight = 1.0f)
        {
            Id = Guid.NewGuid().ToString();
            FromNode = fromNode;
            ToNode = toNode;
            Direction = direction;
            Weight = weight;
        }

        public void Disconnect()
        {
            FromNode.RemoveConnection(this);
            if (Direction == NodeConnectionDirection.Bidirectional)
            {
                ToNode.RemoveConnection(this);
            }
        }
    }
}