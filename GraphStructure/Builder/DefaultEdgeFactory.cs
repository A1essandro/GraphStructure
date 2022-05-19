namespace GraphStructure.Builder
{
    public class DefaultEdgeFactory : IEdgeFactory
    {

        private readonly int _weight;

        public DefaultEdgeFactory(int weight)
        {
            _weight = weight;
        }

        public T CreateEdge<T>(IVertex v1, IVertex v2) where T : IEdge
        {
            if (typeof(T) == typeof(Arc))
                return (T)CreateConcreteArc(v1, v2);

            return (T)CreateConcreteEdge(v1, v2);
        }

        private IEdge CreateConcreteEdge(IVertex v1, IVertex v2) => new Edge(v1, v2, _weight);

        private IEdge CreateConcreteArc(IVertex v1, IVertex v2) => new Arc(v1, v2, _weight);

    }
}
