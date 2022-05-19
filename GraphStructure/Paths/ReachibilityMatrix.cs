using GraphStructure.Extensions;
using System.Collections.Generic;

namespace GraphStructure.Paths
{
    public class ReachibilityMatrix : Matrix<bool>
    {

        public ReachibilityMatrix(params IVertex[] vertices) : this(vertices as IEnumerable<IVertex>)
        {

        }

        public ReachibilityMatrix(IEnumerable<IVertex> vertices) : base(vertices)
        {
            var reachibility = new AdjacencyMatrix(vertices).Convert(x => x ? 1 : 0);
            var forPower = reachibility.Copy();

            for (uint pow = 1; pow < reachibility.Size; pow++)
            {
                var power = forPower.Power(pow);
                reachibility = reachibility.Or(power).Convert(x => x ? 1 : 0);
            }

            foreach (var cell in reachibility)
                this[cell.Key.Item1, cell.Key.Item2] = cell.Value > 0;
        }

    }

}