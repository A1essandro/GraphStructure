using System;
using GraphStructure.Nodes;

namespace GraphStructure.Edges
{
    public class Edge<T> : IEdge<T>
    {

        public Tuple<Node<T>, Node<T>> Nodes { get; }

        public Edge(Node<T> node1, Node<T> node2)
        {
            Nodes = new Tuple<Node<T>, Node<T>>(node1, node2);
        }

        public void Connect()
        {
            Nodes.Item1.AddMaster(Nodes.Item2);
            Nodes.Item1.AddSlave(Nodes.Item2);
            Nodes.Item2.AddMaster(Nodes.Item1);
            Nodes.Item2.AddSlave(Nodes.Item1);
        }
    }
}