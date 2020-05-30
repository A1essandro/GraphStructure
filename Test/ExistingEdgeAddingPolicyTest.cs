using System;
using System.Threading.Tasks;
using GraphStructure.Configuration;
using GraphStructure.Structure;
using GraphStructure.Structure.Edges;
using GraphStructure.Structure.Nodes;
using Xunit;

namespace Test
{
    public class ExistingEdgeAddingPolicyTest
    {


        [Fact]
        public async Task ExceptionTest()
        {
            var node0 = new Node();
            var node1 = new Node();
            var node2 = new Node();
            var arc = new Arc(node0, node1);
            var edge = new Edge(node1, node2);
            var graph = new Graph(new GraphConfiguration()
            {
                ExistingEdgeAddingPolicy = ExistingEdgeAddingPolicy.Exception
            });
            graph.Add(arc).Add(edge);

            Assert.Equal(2, graph.Edges.Count);
            graph.Add(new Arc(node1, node0));
            Assert.Equal(3, graph.Edges.Count);
            await Assert.ThrowsAsync<ArgumentException>(() => graph.AddAsync(new Edge(node0, node1)));
            Assert.Throws<ArgumentException>(() => graph.Add(new Edge(node1, node0)));
        }

        [Fact]
        public async Task IgnoreTest()
        {
            var node0 = new Node();
            var node1 = new Node();
            var node2 = new Node();
            var arc = new Arc(node0, node1);
            var edge = new Edge(node1, node2);
            var graph = new Graph(new GraphConfiguration()
            {
                ExistingEdgeAddingPolicy = ExistingEdgeAddingPolicy.Ignore
            });
            graph.Add(arc).Add(edge);

            Assert.Equal(2, graph.Edges.Count);
            graph.Add(new Arc(node1, node0));
            Assert.Equal(3, graph.Edges.Count);
            await graph.AddAsync(new Edge(node0, node1));
            graph.Add(new Edge(node1, node0));
            Assert.Equal(3, graph.Edges.Count);
        }

        [Fact]
        public async Task AddTest()
        {
            var node0 = new Node();
            var node1 = new Node();
            var node2 = new Node();
            var arc = new Arc(node0, node1);
            var edge = new Edge(node1, node2);
            var graph = new Graph(new GraphConfiguration()
            {
                ExistingEdgeAddingPolicy = ExistingEdgeAddingPolicy.Add
            });
            graph.Add(arc).Add(edge);

            Assert.Equal(2, graph.Edges.Count);
            graph.Add(new Arc(node1, node0));
            Assert.Equal(3, graph.Edges.Count);
            await graph.AddAsync(new Edge(node0, node1));
            Assert.Equal(4, graph.Edges.Count);
            graph.Add(new Edge(node1, node0));
            Assert.Equal(5, graph.Edges.Count);
        }

    }
}