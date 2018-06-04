using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure.Nodes;

namespace GraphStructure
{
    public class Pathfinder<T>
    {

        private readonly Graph<T> _graph;

        public Pathfinder(Graph<T> graph)
        {
            _graph = graph;
        }

        public async Task<IEnumerable<int[]>> GetAllBetween(int from, int to)
        {
            var matrix = await _graph.GetReachibilityMatrix();

            if (matrix[from, to] < 1)
            {
                return new List<int[]>(); //empty
            }

            return default(IEnumerable<int[]>);
        }

        public async Task<IEnumerable<int[]>> GetAllBetween(Node<T> from, Node<T> to)
        {
            var departureIndex = _graph.Nodes.Select(x => x).ToList().IndexOf(from);
            var destinationIndex = _graph.Nodes.Select(x => x).ToList().IndexOf(to);

            return await GetAllBetween(departureIndex, destinationIndex);
        }

    }
}