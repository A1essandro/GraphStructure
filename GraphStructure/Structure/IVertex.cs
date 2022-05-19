using System.Collections.Generic;

namespace GraphStructure
{

    public interface IVertex
    {

        IReadOnlyCollection<IEdge> Edges { get; }

        void AddEdge(IEdge edge);

    }

}
