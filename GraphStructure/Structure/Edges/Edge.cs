using System;
using GraphStructure.Structure.Nodes;

namespace GraphStructure.Structure.Edges
{
    public class Edge<T> : IEdge<T>
    {

        public bool HasDirection => false;

        public Tuple<Node<T>, Node<T>> Nodes { get; }

        public int Weight { get; set; }

        public Edge(Node<T> node1, Node<T> node2, int weight = 1)
        {
            Weight = weight;
            Nodes = new Tuple<Node<T>, Node<T>>(node1, node2);
        }

        public bool IsHasSameDirectionWith(IEdge<T> edge)
        {
            bool check() => (Nodes.Item1 == edge.Nodes.Item1 && Nodes.Item2 == edge.Nodes.Item2)
                            || (Nodes.Item2 == edge.Nodes.Item1 && Nodes.Item1 == edge.Nodes.Item2);

            if (edge.HasDirection)
            {
                return check();
            }

            else return check() || (Nodes.Item1 == edge.Nodes.Item2 && Nodes.Item2 == edge.Nodes.Item1);
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