using System;
using GraphStructure.Structure.Nodes;

namespace GraphStructure.Structure.Edges
{
    public class Edge<T> : IEdge<T>
    {

        public Tuple<Node<T>, Node<T>> Nodes { get; }

        public int Cost { get; }

        public int Weight => Cost;

        public Edge(Node<T> node1, Node<T> node2, int cost = 1)
        {
            Cost = cost;
            Nodes = new Tuple<Node<T>, Node<T>>(node1, node2);
        }

        public void Connect()
        {
            Nodes.Item1.AddMaster(Nodes.Item2);
            Nodes.Item1.AddSlave(Nodes.Item2);
            Nodes.Item2.AddMaster(Nodes.Item1);
            Nodes.Item2.AddSlave(Nodes.Item1);
        }

        public void Disconnect()
        {
            Nodes.Item1.RemoveMaster(Nodes.Item2);
            Nodes.Item1.RemoveSlave(Nodes.Item2);
            Nodes.Item2.RemoveMaster(Nodes.Item1);
            Nodes.Item2.RemoveSlave(Nodes.Item1);
        }

    }

    public class Edge : Edge<object>
    {
        public Edge(Node<object> node1, Node<object> node2, int cost = 1)
            : base(node1, node2, cost)
        {
        }
    }

}