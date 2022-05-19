using System.Linq;

namespace GraphStructure.Builder
{
    internal sealed class PathVertexBuilder : PathBuilder, IVertexBuilder
    {

        private readonly IVertex _vertex;

        public PathVertexBuilder(Graph graph)
            : base(graph)
        {
        }

        public IVertexBuilder ConnectWith(IVertex vertex, int weight = 0)
        {
            TryAddVertex(vertex);
            var edge = new Edge(_vertex, vertex, weight);

            vertex.AddEdge(edge);
            _vertex.AddEdge(edge);

            return this;
        }

        public IVertexBuilder ConnectWith<T>(IVertex vertex, IEdgeFactory factory) where T : IEdge
        {
            TryAddVertex(vertex);
            var edge = factory.CreateEdge<T>(_vertex, vertex);

            vertex.AddEdge(edge);
            _vertex.AddEdge(edge);

            return this;
        }

        public IVertexBuilder ThenConnectWith(IVertex vertex, int weight = 0)
        {
            TryAddVertex(vertex);
            var edge = new Edge(_vertex, vertex, weight);

            vertex.AddEdge(edge);
            _vertex.AddEdge(edge);

            return new VertexBuilder(vertex, Graph);
        }

        public IVertexBuilder ThenConnectWith<T>(IVertex vertex, IEdgeFactory factory) where T : IEdge
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
