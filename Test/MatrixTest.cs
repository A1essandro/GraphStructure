using System.Linq;
using System.Threading.Tasks;
using GraphStructure;
using GraphStructure.Edges;
using GraphStructure.Nodes;
using Xunit;

namespace Test
{
    public class MatrixTest
    {

        private Graph<int> _getGraph()
        {
            var graph = new Graph<int>();
            graph.Add(new Node<int>())
                .Add(new Node<int>())
                .Add(new Node<int>())
                .Add(new Node<int>());

            graph.Add(new Arc<int>(graph.Nodes.ElementAt(0), graph.Nodes.ElementAt(0))) //to himself
                .Add(new Arc<int>(graph.Nodes.ElementAt(0), graph.Nodes.ElementAt(1)))
                .Add(new Arc<int>(graph.Nodes.ElementAt(1), graph.Nodes.ElementAt(2)))
                .Add(new Edge<int>(graph.Nodes.ElementAt(2), graph.Nodes.ElementAt(3)));

            return graph;
        }

        [Fact]
        public async Task AdjacencyTest()
        {
            var graph = _getGraph();
            var matrix = await graph.GetAdjacencyMatrix();

            var expectation = new int[,] {
                { 1, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 },
                { 0, 0, 1, 0 },
            };

            Assert.Equal(expectation, matrix);
        }

        [Fact]
        public async Task ReachibilityTest()
        {
            var graph = _getGraph();
            var matrix = await graph.GetReachibilityMatrix();

            var expectation = new int[,] {
                { 1, 1, 1, 1 },
                { 0, 0, 1, 1 },
                { 0, 0, 1, 1 },
                { 0, 0, 1, 1 },
            };

            Assert.Equal(expectation, matrix);
        }

    }
}