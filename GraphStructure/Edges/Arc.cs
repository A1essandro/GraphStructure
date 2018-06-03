using System;
using GraphStructure.Nodes;

namespace GraphStructure.Edges
{
    public class Arc<T> : IEdge<T>
    {
        public Tuple<Node<T>, Node<T>> Nodes { get; }

        public Arc(Node<T> node1, Node<T> node2)
        {
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
}