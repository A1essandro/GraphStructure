using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure.Structure.Nodes;
using GraphStructure.Structure;
using Nito.AsyncEx;

namespace GraphStructure.Paths
{

    public class Pathfinder<T>
    {

        private readonly Graph<T> _graph;
        private readonly MatrixCalculator<T> _matrixCalculator;

        public Pathfinder(Graph<T> graph)
        {
            _graph = graph;
            _matrixCalculator = new MatrixCalculator<T>(_graph);
        }

        /// <summary>
        /// Search for all possible paths (without loops) between two nodes
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Path<T>>> GetAllBetween(Node<T> from, Node<T> to)
        {
            var reachibilityMatrix = await _matrixCalculator.GetReachibilityMatrix();
            var adjacencyMatrix = await _matrixCalculator.GetAdjacencyMatrix();

            var rawResult = new List<List<Node<T>>>();
            var result = new List<Path<T>>();
            if (reachibilityMatrix[from, to] < 1)
            {
                return new List<Path<T>>(); //empty
            }
            rawResult.Add(new List<Node<T>> { from });
            result.Add(new Path<T>(from));

            return await _fork(to, result, reachibilityMatrix, adjacencyMatrix);
        }

        public async Task<bool> HasPathBetween(Node<T> from, Node<T> to)
        {
            var reachibilityMatrix = await _matrixCalculator.GetReachibilityMatrix();

            return reachibilityMatrix[from, to] > 0;
        }

        private async Task<IList<Path<T>>> _fork(Node<T> destination, IList<Path<T>> pathes,
                                    Matrix<T> reachibilityMatrix, Matrix<T> adjacencyMatrix)
        {
            var forked = new List<Path<T>>();

            var uncompleted = pathes.AsParallel().Where(p => !p.Contains(destination));
            uncompleted.ForAll(async path =>
            {
                var rwLock = new AsyncReaderWriterLock();
                var lastStep = path.Last();
                var nextStepsCandidates = await adjacencyMatrix.GetRow(lastStep);
                var nextSteps = nextStepsCandidates.AsParallel().Where(x => x.Value > 0).Select(x => x.Key)
                    .Where(candidate => reachibilityMatrix[candidate, destination] > 0 || candidate == destination);

                foreach (var nextStep in nextSteps.Where(c => !path.Contains(c)).AsEnumerable())
                {
                    var pathCopy = await path.Clone();
                    await pathCopy.Add(nextStep);
                    using (await rwLock.WriterLockAsync()) forked.Add(pathCopy);
                }
            });

            if (!forked.Any())
            {
                return pathes;
            }

            var completedPathes = pathes.Where(path => path.Contains(destination));
            forked.AddRange(completedPathes);

            return await _fork(destination, forked, reachibilityMatrix, adjacencyMatrix).ConfigureAwait(false); //tail-recursive
        }

    }
}