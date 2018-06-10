using System;
using GraphStructure.Structure.Nodes;

namespace GraphStructure.Structure.Edges
{
    public class Arc<T> : IEdge<T>
    {
        public Tuple<Node<T>, Node<T>> Nodes { get; }

        public int Cost { get; }

        public int Weight => Cost;

        public Arc(Node<T> node1, Node<T> node2, int cost = 1)
        {
            Cost = cost;
            Nodes = new Tuple<Node<T>, Node<T>>(node1, node2);
        }

        public void Connect()
        {
            Nodes.Item1.AddSlave(Nodes.Item2);
            Nodes.Item2.AddMaster(Nodes.Item1);
        }

        public void Disconnect()
        {
            Nodes.Item1.RemoveSlave(Nodes.Item2);
            Nodes.Item2.RemoveMaster(Nodes.Item1);
        }

    }

    public class Arc : Arc<object>
    {
        public Arc(Node<object> node1, Node<object> node2, int cost = 1)
            : base(node1, node2, cost)
        {
        }
    }

}