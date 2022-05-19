using System.Linq;

namespace GraphStructure.Builder
{

    public class VertexBuilder : GraphBuilder, IVertexBuilder
    {

        private readonly IVertex _vertex;

        public VertexBuilder(IVertex vertex, Graph graph) : base(graph)
        {
            _vertex = vertex;
        }

        public virtual IVertexBuilder ConnectWith(IVertex vertex, int weight = 0)
        {
            TryAddVertex(vertex);
            var edge = new Edge(_vertex, vertex, weight);

            vertex.AddEdge(edge);
            _vertex.AddEdge(edge);

            return new VertexBuilder(vertex, Graph);
        }

        public virtual IVertexBuilder ConnectWith<T>(IVertex vertex, IEdgeFactory factory) where T : IEdge
        {
            TryAddVertex(vertex);
            var edge = factory.CreateEdge<T>(_vertex, vertex);

            vertex.AddEdge(edge);
            _vertex.AddEdge(edge);

            return new VertexBuilder(vertex, Graph);
        }

        private void TryAddVertex(IVertex vertex)
        {
            if (!Graph.vertices.Contains(vertex))
                AddVertex(vertex);
        }
    }
}
