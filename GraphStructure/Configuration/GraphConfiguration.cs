namespace GraphStructure.Configuration
{
    public class GraphConfiguration
    {

        public static GraphConfiguration Default => new GraphConfiguration();

        public ExistingEdgeAddingPolicy ExistingEdgeAddingPolicy { get; set; } = ExistingEdgeAddingPolicy.Exception;

    }
}