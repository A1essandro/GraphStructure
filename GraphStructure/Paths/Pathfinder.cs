using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GraphStructure.Paths
{

    public class Pathfinder
    {

        private readonly Graph _graph;

        public Pathfinder(Graph graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Search for all possible paths (without loops) between two IVertexs
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public IEnumerable<Path> GetAllBetween(IVertex from, IVertex to)
        {
            var reachibilityMatrix = new ReachibilityMatrix(_graph.Vertices);
            var adjacencyMatrix = new AdjacencyMatrix(_graph.Vertices);

            if (!reachibilityMatrix[from, to])
            {
                return Array.Empty<Path>(); //empty
            }

            var forked = new ConcurrentBag<Path>(InitialFork(to, reachibilityMatrix, adjacencyMatrix, from));
            return Fork(to, forked, reachibilityMatrix, adjacencyMatrix);
        }

        private IEnumerable<Path> Fork(IVertex destination, IEnumerable<Path> pathes, Matrix<bool> reachibilityMatrix, Matrix<bool> adjacencyMatrix)
        {
            var forked = pathes.SelectMany(path => Forking(destination, reachibilityMatrix, adjacencyMatrix, path)).ToArray();

            if (forked.Sum(x => x.Length) == pathes.Sum(x => x.Length))
            {
                return pathes;
            }

            return Fork(destination, forked, reachibilityMatrix, adjacencyMatrix); //tail-recursive
        }

        private static IEnumerable<Path> Forking(IVertex destination, Matrix<bool> reachibilityMatrix, Matrix<bool> adjacencyMatrix, Path path)
        {
            var lastVertex = path.Last().End;

            if (lastVertex == destination)
                yield return path;
            else
            {
                var nextStepsCandidates = adjacencyMatrix.GetRow(lastVertex);
                var nextVertices = nextStepsCandidates.AsParallel().Where(x => x.Value).Select(x => x.Key)
                    .Where(x => !path.Contains(x))
                    .Where(candidate => reachibilityMatrix[candidate, destination] || candidate == destination)
                    .ToArray();

                var newSteps = lastVertex.Edges.SelectMany(x => x.Directions).Where(x => nextVertices.Contains(x.End));
                foreach (var nextStep in newSteps)
                {
                    var pathCopy = path.Clone();
                    pathCopy.Add(nextStep);
                    yield return pathCopy;
                }
            }
        }

        private static IEnumerable<Path> InitialFork(IVertex destination, Matrix<bool> reachibilityMatrix, Matrix<bool> adjacencyMatrix, IVertex startVertex)
        {
            var lastVertex = startVertex;
            var nextStepsCandidates = adjacencyMatrix.GetRow(lastVertex);
            var nextVertices = nextStepsCandidates.AsParallel().Where(x => x.Value).Select(x => x.Key)
                .Where(candidate => reachibilityMatrix[candidate, destination] || candidate == destination)
                .ToArray();

            var newSteps = lastVertex.Edges.SelectMany(x => x.Directions).Where(x => nextVertices.Contains(x.End));
            foreach (var nextStep in newSteps)
            {
                yield return new Path(nextStep);
            }
        }
    }
}