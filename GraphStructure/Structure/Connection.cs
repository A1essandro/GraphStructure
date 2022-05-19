using System;
using System.Collections.Generic;

namespace GraphStructure
{

    public abstract class Connection : IEdge
    {

        protected Connection(IVertex a, IVertex b, double weight = 0)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (b == null) throw new ArgumentNullException(nameof(b));

            Vertices = (a, b);
            Weight = weight;
        }

        public double Weight { get; set; } = 0;

        public (IVertex, IVertex) Vertices { get; set; }

        public abstract bool IsDirected { get; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is IEdge edge) return edge.GetHashCode() == GetHashCode();

            return false;
        }

        public IReadOnlyCollection<IDirection> Directions => IsDirected
            ? new[] { new Direction(Vertices.Item1, Vertices.Item2, this, Weight) }
            : new[] { new Direction(Vertices.Item1, Vertices.Item2, this, Weight), new Direction(Vertices.Item2, Vertices.Item1, this, Weight) };

        public override int GetHashCode()
        {
            var dirrectedFactor = IsDirected ? 3 : 7;
            return Weight.GetHashCode() ^ (Vertices.Item1.GetHashCode() + Vertices.Item2.GetHashCode()) ^ dirrectedFactor;
        }

        public static bool operator ==(Connection edge1, Connection edge2) => edge1.Equals(edge2);

        public static bool operator !=(Connection edge1, Connection edge2) => !edge1.Equals(edge2);

    }

}
