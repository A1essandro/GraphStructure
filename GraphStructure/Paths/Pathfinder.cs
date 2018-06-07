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

        public Pathfinder(Graph<T> graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Search for all possible paths (without loops) between two nodes
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Path<T>>> GetAllBetween(int from, int to)
        {
            var nodeList = _graph.Nodes;
            var reachibilityMatrix = await _graph.GetReachibilityMatrix();
            var adjacencyMatrix = await _graph.GetAdjacencyMatrix();

            var rawResult = new List<List<int>>();
            if (reachibilityMatrix[from, to] < 1)
            {
                return new List<Path<T>>(); //empty
            }
            rawResult.Add(new List<int> { from });

            rawResult = await _fork(to, rawResult, reachibilityMatrix, adjacencyMatrix);
            var paths = rawResult.AsParallel().Select(steps => new Path<T>(steps.Select(step => nodeList.ElementAt(step))));

            return paths.AsEnumerable();
        }

        /// <summary>
        /// Search for all possible paths (without loops) between two nodes
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Path<T>>> GetAllBetween(Node<T> from, Node<T> to)
        {
            var departureIndex = _graph.Nodes.Select(x => x).ToList().IndexOf(from);
            var destinationIndex = _graph.Nodes.Select(x => x).ToList().IndexOf(to);

            return await GetAllBetween(departureIndex, destinationIndex);
        }

        private async Task<List<List<int>>> _fork(int destination, List<List<int>> pathes,
                                    int[,] reachibilityMatrix, int[,] adjacencyMatrix)
        {
            var forked = new List<List<int>>();

            await Task.Run(() => pathes.AsParallel().Where(p => !p.Contains(destination)).ForAll(async path =>
            {
                var rwLock = new AsyncReaderWriterLock();
                var lastStep = path.Last();
                var nextStepsCandidates = adjacencyMatrix.GetRow(lastStep).GetPositiveIndexes();
                var nextSteps = nextStepsCandidates
                    .Where(candidate => reachibilityMatrix[candidate, destination] > 0 || candidate == destination);

                foreach (var nextStep in nextSteps.Where(c => !path.Contains(c)))
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