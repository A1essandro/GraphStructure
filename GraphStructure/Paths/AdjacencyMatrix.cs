using System.Collections.Generic;
using System.Linq;

namespace GraphStructure.Paths
{
    public class AdjacencyMatrix : Matrix<bool>
    {

        public AdjacencyMatrix(params IVertex[] vertices) : this(vertices as IEnumerable<IVertex>)
        {

        }

        public AdjacencyMatrix(IEnumerable<IVertex> vertices) : base(vertices)
        {
            foreach (var cell in this)
            {
                var x = cell.Key.Item1;
                var y = cell.Key.Item2;
                this[x, y] = GetAdjacencyVertices(x).Contains(y);
            }
        }

        private IEnumerable<IVertex> GetAdjacencyVertices(IVertex vertex)
        {
            return vertex.Edges.SelectMany(x => x.Directions)
                .Where(x => ReferenceEquals(x.Start, vertex))
                .Select(x => x.End).Distinct();
        }

    }

}