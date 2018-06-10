using System;
using GraphStructure.Structure.Nodes;

namespace GraphStructure.Structure.Edges
{
    public interface IEdge<T>
    {

        int Cost { get; }

        int Weight { get; }

        Tuple<Node<T>, Node<T>> Nodes { get; }

        void Connect();

        void Disconnect();

    }
}