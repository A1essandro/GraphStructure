using System.Collections.Generic;

namespace GraphStructure
{

    public interface IEdge
    {

        double Weight { get; set; }

        (IVertex, IVertex) Vertices { get; }

        bool IsDirected { get; }

        IReadOnlyCollection<IDirection> Directions { get; }

    }

}
