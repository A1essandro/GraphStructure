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

        public async Task<Matrix<T>> GetAdjacencyMatrix()
        {
            var matrix = new Matrix<T>(_graph.Nodes);

            await Task.Run(() =>
            {
                foreach (var x in _graph.Nodes)
                {
                    foreach (var y in _graph.Nodes)
                    {
                        matrix[x, y] = x.SlaveNodes.Contains(y) ? 1 : 0;
                    }
                }
            });

            return matrix;
        }

        public async Task<Matrix<T>> GetReachibilityMatrix()
        {
            return await Task.Run(async () =>
            {
                var result = await GetAdjacencyMatrix();
                var toPower = await GetAdjacencyMatrix();
                var len = toPower.Size;

                for (uint pow = 1; pow < len; pow++)
                {
                    var power = await Matrix<T>.Power(toPower, pow);
                    result = await Matrix<T>.Or(result, power);
                }

                return result;
            });
        }

    }
}