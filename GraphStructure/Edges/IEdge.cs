using System;
using GraphStructure.Nodes;

namespace GraphStructure.Edges
{
    public interface IEdge<T>
    {
         
        Tuple<Node<T>, Node<T>> Nodes { get; }

        void Connect();

        void Disconnect();

    }
}