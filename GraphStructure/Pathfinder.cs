using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure.Common;
using GraphStructure.Nodes;
using Nito.AsyncEx;

namespace GraphStructure
{
    public class Pathfinder<T>
    {

        private readonly Graph<T> _graph;

        public Pathfinder(Graph<T> graph)
        {
            _graph = graph;
        }

        public async Task<IEnumerable<List<int>>> GetAllBetween(int from, int to)
        {
            var reachibilityMatrix = await _graph.GetReachibilityMatrix();
            var adjacencyMatrix = await _graph.GetAdjacencyMatrix();

            var result = new List<List<int>>();
            if (reachibilityMatrix[from, to] < 1)
            {
                return result; //empty
            }
            result.Add(new List<int> { from });

            return await _fork(to, result, reachibilityMatrix, adjacencyMatrix);
        }

        public async Task<IEnumerable<List<int>>> GetAllBetween(Node<T> from, Node<T> to)
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