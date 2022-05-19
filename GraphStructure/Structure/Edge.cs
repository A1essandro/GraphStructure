namespace GraphStructure
{

    public class Edge : Connection
    {

        public Edge(IVertex a, IVertex b, double weight = 0) : base(a, b, weight)
        {
        }

        public override bool IsDirected => false;

    }

}
