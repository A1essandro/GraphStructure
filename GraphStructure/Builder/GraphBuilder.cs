using System;
using System.Linq;

namespace GraphStructure.Builder
{


    public class GraphBuilder : IGraphBuilder
    {

        protected readonly Graph Graph;

        public GraphBuilder(Graph graphContext)
        {
            Graph = graphContext;
        }

        public virtual IVertexBuilder AddVertex(IVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));
            if (Graph.vertices.Contains(vertex)) throw new ArgumentException("Vertex already exists in this graph");
            if (vertex.Edges.Any())
            {
                AddAllRefferences(vertex);
            }

            Graph.vertices.Add(vertex);

            return new VertexBuilder(vertex, Graph);
        }

        private void AddAllRefferences(IVertex vertex)
        {
            foreach (var edge in vertex.Edges)
            {
                if (edge.Vertices.Item1 == null || edge.Vertices.Item2 == null) throw new ArgumentException("Edge should contain 2 vertices");

                var anotherVertex = ReferenceEquals(edge.Vertices.Item1, vertex) ? edge.Vertices.Item2 : edge.Vertices.Item1;
                AddVertex(anotherVertex);
            }
        }
    }
}
