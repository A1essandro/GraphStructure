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
            var node = new Node<object>();
            var arc = new Arc<object>(node, new Node<object>());
            var edge = new Edge<object>(new Node<object>(), node);
            var graph = new Graph<object>(arc, edge);

            Assert.Equal(2, graph.Edges.Count);
            Assert.Equal(3, graph.Nodes.Count);
            Assert.Equal(3, graph.Order);
        }

        [Fact]
        public void AdditionTest()
        {
            var node0 = new Node<object>();
            var node1 = new Node<object>();
            var node2 = new Node<object>();
            var arc = new Arc<object>(node0, node1);
            var edge = new Edge<object>(node1, node2);
            var graph = new Graph<object>(arc, edge);

            Assert.Equal(2, graph.Edges.Count);
            graph.Add(new Arc<object>(node1, node0));
            Assert.Equal(3, graph.Edges.Count);
            Assert.ThrowsAsync<ArgumentException>(async () => await graph.AddAsync(new Edge<object>(node0, node1)));
            Assert.Throws<ArgumentException>(() => graph.Add(new Edge<object>(node1, node0)));
        }

        [Fact]
        public async Task RemoveTest()
        {
            var arc0 = new Arc<object>(new Node<object>(), new Node<object>());
            var edge0 = new Edge<object>(new Node<object>(), new Node<object>());
            var arc1 = new Arc<object>(new Node<object>(), new Node<object>());
            var edge1 = new Edge<object>(new Node<object>(), new Node<object>());

            var graph = new Graph<object>(arc0, edge0, arc1, edge1);
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

    }
}