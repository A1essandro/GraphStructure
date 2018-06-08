using System.Linq;
using System.Threading.Tasks;
using GraphStructure;
using GraphStructure.Paths;
using GraphStructure.Structure;
using GraphStructure.Structure.Edges;
using GraphStructure.Structure.Nodes;
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
            var calculator = new MatrixCalculator<int>(graph);
            var matrix = await calculator.GetAdjacencyMatrix();

            var node0 = graph.Nodes.ElementAt(0);
            var node1 = graph.Nodes.ElementAt(1);
            var node2 = graph.Nodes.ElementAt(2);
            var node3 = graph.Nodes.ElementAt(3);

            // { 1, 1, 0, 0 },
            // { 0, 0, 1, 0 },
            // { 0, 0, 0, 1 },
            // { 0, 0, 1, 0 },

            Assert.True(matrix[node0, node0] > 0);
            Assert.True(matrix[node3, node2] > 0);
            Assert.True(matrix[node0, node2] == 0);
            Assert.True(matrix[node1, node2] == 0);
        }

        [Fact]
        public async Task ReachibilityTest()
        {
            var graph = _getGraph();
            var calculator = new MatrixCalculator<int>(graph);
            var matrix = await calculator.GetReachibilityMatrix();

            var node0 = graph.Nodes.ElementAt(0);
            var node1 = graph.Nodes.ElementAt(1);
            var node2 = graph.Nodes.ElementAt(2);
            var node3 = graph.Nodes.ElementAt(3);

            // { 1, 1, 1, 1 },
            // { 0, 0, 1, 1 },
            // { 0, 0, 1, 1 },
            // { 0, 0, 1, 1 },

            Assert.True(matrix[node0, node0] > 0);
            Assert.True(matrix[node0, node2] > 0);
            Assert.True(matrix[node0, node3] > 0);
            Assert.True(matrix[node1, node1] == 0);
            Assert.True(matrix[node3, node0] == 0);
        }

    }
}