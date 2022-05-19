namespace GraphStructure.Builder
{
    public interface IEdgeFactory
    {

        T CreateEdge<T>(IVertex v1, IVertex v2) where T : IEdge;

    }
}
