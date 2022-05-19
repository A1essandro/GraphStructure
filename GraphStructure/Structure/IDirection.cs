namespace GraphStructure
{
    public interface IDirection
    {
        IEdge Edge { get; }
        IVertex End { get; }
        IVertex Start { get; }
        double Weight { get; }
    }
}