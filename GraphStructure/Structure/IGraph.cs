using System.Collections.Generic;

namespace GraphStructure
{
    public interface IGraph
    {

        IReadOnlyCollection<IVertex> Vertices { get; }

        IReadOnlyCollection<IEdge> Edges { get; }

        int Size { get; }

        int Order { get; }

    }

}
