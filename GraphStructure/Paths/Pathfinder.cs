using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure.Structure.Nodes;
using GraphStructure.Structure;
using Nito.AsyncEx;
using GraphStructure.Common;

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
            var nodeList = _graph.Nodes;
            var reachibilityMatrix = await _matrixCalculator.GetReachibilityMatrix();
            var adjacencyMatrix = await _matrixCalculator.GetAdjacencyMatrix();

            var rawResult = new List<List<Node<T>>>();
            if (reachibilityMatrix[from, to] < 1)
            {
                return new List<Path<T>>(); //empty
            }
            rawResult.Add(new List<Node<T>> { from });

            rawResult = await _fork(to, rawResult, reachibilityMatrix, adjacencyMatrix);
            var paths = rawResult.AsParallel().Select(steps => new Path<T>(steps));

            return paths.AsEnumerable();
        }

        private async Task<List<List<Node<T>>>> _fork(Node<T> destination, List<List<Node<T>>> pathes,
                                    Matrix<T> reachibilityMatrix, Matrix<T> adjacencyMatrix)
        {
            var forked = new List<List<Node<T>>>();

            var uncompleted = pathes.AsParallel().Where(p => !p.Contains(destination));
            await Task.Run(() => uncompleted.ForAll(async path =>
            {
                var rwLock = new AsyncReaderWriterLock();
                var lastStep = path.Last();
                var nextStepsCandidates = await adjacencyMatrix.GetRow(lastStep);
                var nextSteps = nextStepsCandidates.AsParallel().Where(x => x.Value > 0).Select(x => x.Key)
                    .Where(candidate => reachibilityMatrix[candidate, destination] > 0 || candidate == destination);

                foreach (var nextStep in nextSteps.Where(c => !path.Contains(c)).AsEnumerable())
                {
                    var pathCopy = path.ToList();
                    pathCopy.Add(nextStep);
                    using (await rwLock.WriterLockAsync()) forked.Add(pathCopy);
                }
            }));

            if (forked.Count() == 0)
            {
                return pathes;
            }

            var completedPathes = pathes.Where(path => path.Contains(destination));
            forked.AddRange(completedPathes);

            return await _fork(destination, forked, reachibilityMatrix, adjacencyMatrix).ConfigureAwait(false); //tail-recursive
        }

    }
}