namespace GraphStructure.Builder
{

    public interface IVertexBuilder : IGraphBuilder
    {

        IVertexBuilder ConnectWith(IVertex vertex, int weight = 0);

        IVertexBuilder ConnectWith<T>(IVertex vertex, IEdgeFactory factory) where T : IEdge;

    }
}
