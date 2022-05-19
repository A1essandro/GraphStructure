namespace GraphStructure
{

    public class Direction : IDirection
    {

        public Direction(IVertex start, IVertex end, IEdge edge, double weight = 0)
        {
            Start = start;
            End = end;
            Edge = edge;
            Weight = weight;
        }

        public IEdge Edge { get; }

        public IVertex Start { get; }

        public IVertex End { get; }

        public double Weight { get; }

    }

}
