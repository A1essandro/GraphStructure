using System;
using GraphStructure.Nodes;

namespace GraphStructure.Edges
{
    public interface IEdge<T>
    {

        int Cost { get; }

        Tuple<Node<T>, Node<T>> Nodes { get; }

        void Connect();

        void Disconnect();

    }
}