using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure;
using GraphStructure.Edges;
using GraphStructure.Nodes;
using Xunit;

namespace Test
{
    public class PathfinderTest
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
                .Add(new Edge<int>(graph.Nodes.ElementAt(2), graph.Nodes.ElementAt(3)))
                .Add(new Arc<int>(graph.Nodes.ElementAt(3), graph.Nodes.ElementAt(0)))
                .Add(new Edge<int>(graph.Nodes.ElementAt(1), graph.Nodes.ElementAt(3)));

            return graph;
        }

        [Fact]
        public async Task AllBetweenTest()
        {
            var graph = _getGraph();
            var pathfinder = new Pathfinder<int>(graph);

            var pathes = await pathfinder.GetAllBetween(graph.Nodes.ElementAt(0), graph.Nodes.ElementAt(3));

            Assert.Equal(2, pathes.Count());
            Assert.Contains<List<int>>(new List<int> { 0, 1, 3 }, pathes);
            Assert.Contains<List<int>>(new List<int> { 0, 1, 2, 3 }, pathes);
        }

    }
}