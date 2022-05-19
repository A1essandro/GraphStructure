namespace GraphStructure.Builder
{
    internal class PathBuilder : GraphBuilder
    {

        protected IVertex LastVertex;

        public PathBuilder(Graph graphContext)
            : base(graphContext)
        {
        }

        public override IVertexBuilder AddVertex(IVertex vertex)
        {
            base.AddVertex(vertex);
            LastVertex = vertex;

            return new PathVertexBuilder(Graph);
        }

    }
}
