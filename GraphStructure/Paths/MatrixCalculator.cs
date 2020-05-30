using GraphStructure.Structure;
using GraphStructure.Structure.Edges;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GraphStructure.Paths
{
    public sealed class MatrixCalculator<T>
    {
        private readonly Graph<T> _graph;
        private Tuple<int, Matrix<T>> _adjacencyCache = new Tuple<int, Matrix<T>>(0, null);
        private Tuple<int, Matrix<T>> _reachibilityCache = new Tuple<int, Matrix<T>>(0, null);
        private Tuple<int, Matrix<T, int?>> _edgeWeightsCache = new Tuple<int, Matrix<T, int?>>(0, null);

        public MatrixCalculator(Graph<T> graph)
        {
            _graph = graph;
        }

        public async Task<Matrix<T>> GetAdjacencyMatrix()
        {
            if (_adjacencyCache.Item1 == _graph.GetHashCode())
            {
                return _adjacencyCache.Item2;
            }

            var matrix = new Matrix<T>(_graph.Nodes);
            foreach (var node in _graph.Nodes)
            {
                var slaveNodes = await _graph.GetSlaveNodesFor(node);
                foreach (var slave in slaveNodes)
                {
                    matrix[node, slave] = 1;
                }
            }

            _adjacencyCache = new Tuple<int, Matrix<T>>(_graph.GetHashCode(), matrix);

            return matrix;
        }

        public async Task<Matrix<T>> GetReachibilityMatrix()
        {
            if (_reachibilityCache.Item1 == _graph.GetHashCode())
            {
                return _reachibilityCache.Item2;
            }

            var result = await GetAdjacencyMatrix();
            var toPower = await GetAdjacencyMatrix();
            var len = toPower.Size;

            for (uint pow = 1; pow < len; pow++)
            {
                var power = await Matrix<T>.Power(toPower, pow);
                result = await Matrix<T>.Or(result, power);
            }

            _reachibilityCache = new Tuple<int, Matrix<T>>(_graph.GetHashCode(), result);
            return result;
        }

        public async Task<Matrix<T, int?>> GetEdgeWeightsMatrix()
        {
            if (_edgeWeightsCache.Item1 == _graph.GetHashCode())
            {
                return _edgeWeightsCache.Item2;
            }

            IEdge<T> connection;
            var matrix = new Matrix<T, int?>(_graph.Nodes);
            foreach (var x in _graph.Nodes)
            {
                foreach (var y in _graph.Nodes)
                {
                    connection = await _graph.GetConnectionBetween(x, y);
                    matrix[x, y] = connection?.Weight;
                }
            }

            _edgeWeightsCache = new Tuple<int, Matrix<T, int?>>(_graph.GetHashCode(), matrix);

            return matrix;
        }

    }
}