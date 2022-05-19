namespace GraphStructure
{
    public class Arc : Connection
    {

        public Arc(IVertex a, IVertex b, double weight = 0) : base(a, b, weight)
        {
        }

        public override bool IsDirected => true;

        public override int GetHashCode()
        {
            return Weight.GetHashCode() ^ (Vertices.Item1.GetHashCode() * 7) ^ (Vertices.Item2.GetHashCode() * 17);
        }

    }

}
