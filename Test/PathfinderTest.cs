using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure.Paths;
using GraphStructure.Structure;
using GraphStructure.Structure.Edges;
using GraphStructure.Structure.Nodes;
using Xunit;

namespace Test
{
    public class PathfinderTest
    {

        private Graph<string> _getGraph()
        {
            var graph = new Graph<string>();
            graph.Add(new Node<string>("one"))
                .Add(new Node<string>("two"))
                .Add(new Node<string>("three"))
                .Add(new Node<string>("four"));

            graph.Add(new Arc<string>(graph.Nodes.ElementAt(0), graph.Nodes.ElementAt(0))) //to himself
                .Add(new Arc<string>(graph.Nodes.ElementAt(0), graph.Nodes.ElementAt(1)))
                .Add(new Arc<string>(graph.Nodes.ElementAt(1), graph.Nodes.ElementAt(2)))
                .Add(new Edge<string>(graph.Nodes.ElementAt(2), graph.Nodes.ElementAt(3)))
                .Add(new Arc<string>(graph.Nodes.ElementAt(3), graph.Nodes.ElementAt(0)))
                .Add(new Edge<string>(graph.Nodes.ElementAt(1), graph.Nodes.ElementAt(3)));

            return graph;
        }

        [Fact]
        public async Task AllBetweenTest()
        {
            var graph = _getGraph();
            var pathfinder = new Pathfinder<string>(graph);

            var node0 = graph.Nodes.ElementAt(0);
            var node1 = graph.Nodes.ElementAt(1);
            var node2 = graph.Nodes.ElementAt(2);
            var node3 = graph.Nodes.ElementAt(3);

            var pathes = await pathfinder.GetAllBetween(node0, node3);
            var test = pathes.Select(path => path.Select(node => node.Data).ToArray()).ToArray();

            Assert.Equal(2, pathes.Count());
            Assert.True(pathes.First()[0] == node0 && pathes.First().Last() == node3);
            Assert.Contains<string[]>(new string[] { "one", "two", "four" }, test);
            Assert.Contains<List<Node<string>>>(new List<Node<string>> { node0, node1, node2, node3 }, pathes.Select(x => x.ToList()));
            Assert.Equal(3, pathes.Min(x => x.Length));
        }

        [Fact]
        public async Task HasPathTest()
        {
            var graph = new Graph<string>();
            graph.Add(new Node<string>("one"))
                .Add(new Node<string>("two"))
                .Add(new Node<string>("three"))
                .Add(new Node<string>("four"));

            graph.Add(new Arc<string>(graph.Nodes.ElementAt(0), graph.Nodes.ElementAt(1)))
                .Add(new Arc<string>(graph.Nodes.ElementAt(1), graph.Nodes.ElementAt(2)))
                .Add(new Edge<string>(graph.Nodes.ElementAt(2), graph.Nodes.ElementAt(3)))
                .Add(new Edge<string>(graph.Nodes.ElementAt(1), graph.Nodes.ElementAt(3)));

            var pathfinder = new Pathfinder<string>(graph);

            var node0 = graph.Nodes.ElementAt(0);
            var node1 = graph.Nodes.ElementAt(1);
            var node2 = graph.Nodes.ElementAt(2);
            var node3 = graph.Nodes.ElementAt(3);

            Assert.True(await pathfinder.HasPathBetween(node0, node3));
            Assert.True(await pathfinder.HasPathBetween(node0, node2));
            Assert.False(await pathfinder.HasPathBetween(node3, node0));
        }
    }
}