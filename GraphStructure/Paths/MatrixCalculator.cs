using GraphStructure.Common;
using GraphStructure.Structure;
using System.Linq;
using System.Threading.Tasks;

namespace GraphStructure.Paths
{
    public sealed class MatrixCalculator<T>
    {
        private readonly Graph<T> _graph;

        public MatrixCalculator(Graph<T> graph)
        {
            _graph = graph;
        }

        public async Task<int[,]> GetAdjacencyMatrix()
        {
            var matrix = new int[_graph.Order, _graph.Order];
            var nodes = _graph.Nodes;

            await Task.Run(() =>
            {
                for (var x = 0; x < _graph.Order; x++)
                {
                    for (var y = 0; y < _graph.Order; y++)
                    {
                        var hasPath = nodes.ElementAt(x).SlaveNodes.Contains(nodes.ElementAt(y));
                        matrix[x, y] = hasPath ? 1 : 0;
                    }
                }
            });

            return matrix;
        }

        public async Task<Matrix<T, bool>> GetAdjacencyMatrixNew()
        {
            var matrix = new Matrix<T, bool>(_graph.Order);

            await Task.Run(() =>
            {
                foreach (var x in _graph.Nodes)
                {
                    foreach (var y in _graph.Nodes)
                    {
                        var hasPath = x.SlaveNodes.Contains(y);
                        matrix[x, y] = hasPath;
                    }
                }
            });

            return matrix;
        }

        public async Task<int[,]> GetReachibilityMatrix()
        {
            var adjacencyMatrix = await GetAdjacencyMatrix();
            return await Task.Run(() =>
            {
                var len = adjacencyMatrix.GetLength(0);
                var result = (int[,])adjacencyMatrix.Clone();
                var power = (int[,])adjacencyMatrix.Clone();

                for (var pow = 0; pow < len; pow++)
                {
                    result = result.Or(power.Power(pow));
                }

                return result;
            });
        }

    }
}