using GraphStructure.Builder;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test")]

namespace GraphStructure
{

    public class Graph : IGraph
    {

        public Graph(IGraphBuilder builder = null)
        {
            Builder = builder ?? new GraphBuilder(this);
        }

        public IGraphBuilder Builder { get; set; }

        protected internal readonly ConcurrentBag<IVertex> vertices = new ConcurrentBag<IVertex>();
        public IReadOnlyCollection<IVertex> Vertices => vertices.ToArray();

        public IReadOnlyCollection<IEdge> Edges => vertices.SelectMany(x => x.Edges).Distinct().ToArray();

        public int Size => Edges.Count;

        public int Order => vertices.Count;

    }
}
