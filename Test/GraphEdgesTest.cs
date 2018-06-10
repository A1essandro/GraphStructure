using System;
using System.Linq;
using System.Threading.Tasks;
using GraphStructure;
using GraphStructure.Structure;
using GraphStructure.Structure.Edges;
using GraphStructure.Structure.Nodes;
using Xunit;

namespace Test
{
    public class GraphEdgesTest
    {

        [Fact]
        public void ConstructorTest()
        {
            var node = new Node();
            var arc = new Arc(node, new Node());
            var edge = new Edge(new Node(), node, 3);
            var graph = new Graph(arc, edge);

            Assert.Equal(2, graph.Edges.Count);
            Assert.Equal(3, graph.Nodes.Count);
            Assert.Equal(3, graph.Order);
        }

        [Fact]
        public void AdditionTest()
        {
            var node0 = new Node();
            var node1 = new Node();
            var node2 = new Node();
            var arc = new Arc(node0, node1);
            var edge = new Edge(node1, node2);
            var graph = new Graph(arc, edge);

            Assert.Equal(2, graph.Edges.Count);
            graph.Add(new Arc(node1, node0));
            Assert.Equal(3, graph.Edges.Count);
            Assert.ThrowsAsync<ArgumentException>(async () => await graph.AddAsync(new Edge(node0, node1)));
            Assert.Throws<ArgumentException>(() => graph.Add(new Edge(node1, node0)));
        }

        [Fact]
        public async Task RemoveTest()
        {
            var arc0 = new Arc(new Node(), new Node());
            var edge0 = new Edge(new Node(), new Node());
            var arc1 = new Arc(new Node(), new Node());
            var edge1 = new Edge(new Node(), new Node());

            var graph = new Graph(arc0, edge0, arc1, edge1);
            Assert.Equal(4, graph.Size);
            graph.Remove(arc0);
            Assert.Equal(3, graph.Size);
            await graph.RemoveAsync(edge1);
            Assert.Equal(arc1, graph.Edges.ElementAt(1));
            graph.RemoveEdge(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => graph.RemoveEdge(1));
            Assert.Equal(1, graph.Size);
            Assert.Equal(edge0, graph.Edges.ElementAt(0));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await graph.RemoveEdgeAsync(1));
            await graph.RemoveEdgeAsync(0);
            Assert.Equal(0, graph.Size);
        }

        [Fact]
        public async Task GetBetweenTest()
        {
            var node0 = new Node();
            var node1 = new Node();
            var node2 = new Node();
            var arc = new Arc(node0, node1);
            var edge = new Edge(node1, node2);
            var graph = new Graph(arc, edge);

            Assert.Equal(arc, await graph.GetConnectionBetween(node0, node1));
            Assert.Equal(null, await graph.GetConnectionBetween(node1, node0));
            Assert.Equal(edge, await graph.GetConnectionBetween(node1, node2));
            Assert.Equal(edge, await graph.GetConnectionBetween(node2, node1));
        }

    }
}