using System;
using GraphStructure.Structure.Nodes;

namespace GraphStructure.Structure.Edges
{
    public interface IEdge<T>
    {

        int Weight { get; set; }

        bool HasDirection { get; }

        Tuple<Node<T>, Node<T>> Nodes { get; }

        bool IsHasSameDirectionWith(IEdge<T> edge);

    }
}